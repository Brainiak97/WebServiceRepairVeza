using AutoMapper;
using BLL.Models;
using BLL.Services;
using BLL.SignalR;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Security.Claims;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RepairLogApiController : ControllerBase
    {
        private readonly RepairLogService _repairLogService;
        private readonly CommentService _commentService;
        private readonly UserService _userService;
        private readonly IHubContext<RepairLogDetailsHub> _repairLogDetailsHub;
        private readonly IHubContext<RepairLogIndexHub> _repairLogIndexHub;
        private readonly ILogger<RepairLogApiController> _logger;
        private readonly IMapper _mapper;

        public RepairLogApiController(ILogger<RepairLogApiController> logger, IMapper mapper,
            IHubContext<RepairLogIndexHub> repairLogIndexHub, IHubContext<RepairLogDetailsHub> repairLogDetailsHub,
            RepairLogService repairLogService, UserService userService, CommentService commentService)
        {
            _repairLogDetailsHub = repairLogDetailsHub;
            _repairLogIndexHub = repairLogIndexHub;
            _commentService = commentService;
            _userService = userService;
            _repairLogService = repairLogService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allLogs = _mapper.Map<IEnumerable<RepairLogViewModel>>(await _repairLogService.GetItemsWithAttachments());

                return Ok(allLogs.ToList());
            }
            catch
            {
                return NoContent();
            }
        }

        // GET: RepairLogApi/GetItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItemWithAttachments(id));

                return Ok(log);
            }
            catch
            {
                return NotFound(id);
            }
        }

        // POST: RepairLogApi/Create
        [HttpPost]
        public async Task<IActionResult> Create(RepairLogViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            try
            {
                var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                createModel.Author = _mapper.Map<UserViewModel>(await _userService.GetItem(userId));
                createModel.AuthorId = userId;
                createModel.Status = RepairStatus.Request;
                createModel.RequestDate = DateTime.Now;

                var createdLog = _mapper.Map<RepairLogDto>(createModel);
                await _repairLogService.Create(createdLog);
                await _repairLogService.EditGroupsIntoRepairLogAsync(createdLog.Id, createdLog.RepairGroups.Select(_ => _.Name));

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{createdLog.Id} создана",
                    RepairLogId = createdLog.Id,
                    CommentatorId = createdLog.AuthorId,
                };

                await AddAutoComment(comment);

                _logger.LogInformation($"The {nameof(RepairLogDto)} creation was successful.");
                return Ok(createdLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} creation failed.", ex);
                return BadRequest();
            }
        }

        // POST: RepairLogApi/AddAutoComment
        [HttpPost]
        public async Task AddAutoComment(CommentViewModel model)
        {
            try
            {
                var commentMap = _mapper.Map<CommentDto>(model);

                var id = await _repairLogService.AddCommentToLog(model.RepairLogId, commentMap);

                await AddLogNotification_ByRepairGroups(id, model.RepairLogId);

                _logger.LogInformation($"The {nameof(model)} commenting was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(CommentViewModel)} commenting failed.", ex);
            }
        }

        // POST: RepairLogApi/AddLogNotification_ByRepairGroups
        [HttpPost]
        public async Task AddLogNotification_ByRepairGroups(int commId, int logId)
        {
            try
            {
                var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItemWithAttachments(logId));
                var comm = _mapper.Map<CommentViewModel>(await _commentService.GetItem(commId));
                var usersIds = await _userService.GetUsersIdsByRepairGroupsIds(log.RepairGroups.Select(group => group.Id).ToList());

                if (usersIds != null)
                    foreach (var userId in usersIds)
                    {
                        if (comm.CommentatorId != userId)
                            if (log.Executors != null && log.Executors.Select(ex => ex.Id).Contains(userId))
                                await _commentService.AddNotificationsToComment_AtExecutorUser(userId, commId);
                            else
                                await _commentService.AddNotificationsToComment_ByRepairGroups(userId, commId);
                    }

                _logger.LogInformation($"The {nameof(commId)} notifying was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(CommentViewModel)} notifying failed.", ex);
            }
        }

        // POST: RepairLogApi/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(RepairLogViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            try
            {
                var editedLog = _mapper.Map<RepairLogDto>(model);
                await _repairLogService.Update(editedLog);
                await _repairLogService.EditGroupsIntoRepairLogAsync(editedLog.Id, editedLog.RepairGroups.Select(_ => _.Name));

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{editedLog.Id} обновлена",
                    RepairLogId = editedLog.Id,
                    CommentatorId = editedLog.AuthorId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", model.Id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{model.Id}").SendAsync("UpdateLog", model.Id);

                _logger.LogInformation($"The {nameof(RepairLogDto)} editing was successful.");
                return Ok(editedLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} editing failed.", ex);
                return BadRequest();
            }
        }

        // POST: RepairLogApi/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repairLogService.Delete(id);
                _logger.LogInformation($"The {nameof(RepairLogDto)} deleting was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} deleting failed.", ex);
                return BadRequest();
            }
        }

        // POST: RepairLogApi/ToExecute/5
        [HttpPost("{userId}/{logId}")]
        public async Task<IActionResult> ToExecute(int userId, int logId)
        {
            try
            {
                await _repairLogService.AddUserToLogExecutors(logId, userId);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{logId} принята к исполнению",
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} ToExecute was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} ToExecute failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/ToChecking/5
        [HttpPost("{userId}/{logId}")]
        public async Task<IActionResult> ToChecking(int userId, int logId)
        {
            try
            {
                await _repairLogService.ToChecking(logId);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{logId} отправлена на проверку",
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} ToChecking was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} ToChecking failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/ToComplete/5
        [HttpPost("{userId}/{logId}")]
        public async Task<IActionResult> ToComplete(int userId, int logId)
        {
            try
            {
                await _repairLogService.ToComplete(logId);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{logId} выполнена",
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} ToComplete was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} ToComplete failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/ToRevision/5
        [HttpPost("{userId}/{logId}")]
        public async Task<IActionResult> ToRevision(int userId, int logId)
        {
            try
            {
                await _repairLogService.ToExecute(logId);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{logId} отправлена на доработку",
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} ToRevision was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} ToRevision failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/ToRequest/5
        [HttpPost("{userId}/{logId}")]
        public async Task<IActionResult> ToRequest(int userId, int logId)
        {
            try
            {
                await _repairLogService.ToRequest(logId);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Удалены исполнители заявки #{logId}",
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} ToRequest was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} ToRequest failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/Appoint
        [HttpPost]
        public async Task<IActionResult> Appoint(int userId, int logId, List<int> executors)
        {
            try
            {
                await _repairLogService.AddUsersToLogExecutors(logId, executors);

                string commentText = $"Заявкe #{logId} изменены исполнители";
                if (!executors.Any())
                {
                    await ToRequest(userId, logId);
                }

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = commentText,
                    RepairLogId = logId,
                    CommentatorId = userId,
                };

                await AddAutoComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

                _logger.LogInformation($"The {nameof(logId)} Appoint was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} Appoint failed.", ex);
                return NotFound();
            }
        }

        // POST: RepairLogApi/AddCustomComment
        [HttpPost]
        public async Task<IActionResult> AddCustomComment(CommentViewModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                model.Date = DateTime.Now;
                model.CommentatorId = userId;

                var commentMap = _mapper.Map<CommentDto>(model);

                var id = await _repairLogService.AddCommentToLog(model.RepairLogId, commentMap);

                await _commentService.SendNewCommentToActiveUsers(id);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", model.RepairLogId);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{model.RepairLogId}").SendAsync("UpdateLog", model.RepairLogId);

                _logger.LogInformation($"The {nameof(model)} AddCustomComment was successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} AddCustomComment failed.", ex);
                return NotFound();
            }
        }
    }
}
