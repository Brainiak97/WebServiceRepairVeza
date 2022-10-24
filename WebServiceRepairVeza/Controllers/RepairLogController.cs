using AutoMapper;
using BLL.Models;
using BLL.Services;
using BLL.SignalR;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using WebService.ControllerHelpers;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers
{
    [Authorize]
    public class RepairLogController : Controller
    {
        private readonly RepairLogService _repairLogService;
        private readonly CommentService _commentService;
        private readonly UserService _userService;
        private readonly IHubContext<RepairLogDetailsHub> _repairLogDetailsHub;
        private readonly IHubContext<RepairLogIndexHub> _repairLogIndexHub;
        private readonly ILogger<RepairLogService> _logger;
        private readonly IMapper _mapper;

        public RepairLogController(ILogger<RepairLogService> logger, IMapper mapper,
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

        public async Task<IActionResult> Index()
        {
            var allLogs = _mapper.Map<IEnumerable<RepairLogViewModel>>(await _repairLogService.GetCorrespondingLogs(User));

            return View(allLogs.Where(_ => _.Status != RepairStatus.Archive).OrderBy(_ => _.ChangedDate).Reverse());
        }

        public async Task<IActionResult> Archive()
        {
            var allLogs = _mapper.Map<IEnumerable<RepairLogViewModel>>(await _repairLogService.GetCorrespondingLogs(User));

            return View(allLogs.Where(_ => _.Status == RepairStatus.Archive).OrderBy(_ => _.ChangedDate).Reverse());
        }

        public async Task<IActionResult> UpdateIndexPartial(int logId)
        {
            var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItem(logId));

            if (log != null)
                return PartialView("IndexPartial", log);

            return NoContent();
        }

        public async Task<IActionResult> UpdateDatails(int logId)
        {
            var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItem_RepairGroups_Comments_Author(logId));

            if (log != null)
                return PartialView("DetailsPartial", log);

            return NoContent();
        }

        // GET: RepairLog/Create
        [Authorize]
        public IActionResult Create() => View();

        // POST: RepairLog/Create
        [HttpPost]
        public async Task<IActionResult> Create(RepairLogViewModel createModel, List<string> groups)
        {
            if (!ModelState.IsValid || groups.Count == 0)
            {
                return View(createModel);
            }
            try
            {
                var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                createModel.Author = _mapper.Map<UserViewModel>(await _userService.GetItem(userId));
                createModel.AuthorId = userId;
                createModel.Status = RepairStatus.Request;
                createModel.RequestDate = DateTime.Now;
                createModel.ChangedDate = createModel.RequestDate;

                var createdLog = _mapper.Map<RepairLogDto>(createModel);
                await _repairLogService.Create(createdLog);
                await _repairLogService.EditGroupsIntoRepairLogAsync(createdLog.Id, groups);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{createdLog.Id} создана",
                    RepairLogId = createdLog.Id,
                    CommentatorId = createdLog.AuthorId,
                };

                await AddServerComment(comment);

                var createdLogMap = _mapper.Map<RepairLogViewModel>(createdLog);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("CreateLog", await ControllerExtensions.RenderViewAsync(this, "IndexPartial", createdLogMap, true));

                _logger.LogInformation($"The {nameof(RepairLogDto)} creation was successful.");
                return RedirectToAction(nameof(Index), new { page = "logs" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} creation failed.", ex);
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = "Ошибка создания заявок" });
            }
        }

        // GET: RepairLog/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItem_RepairGroups_Comments_Author(id));
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();

            return View(log);
        }

        // POST: RepairLog/Edit/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(RepairLogViewModel model, List<string> groups, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var editedLog = _mapper.Map<RepairLogDto>(model);
                await _repairLogService.Update(editedLog);
                await _repairLogService.EditGroupsIntoRepairLogAsync(editedLog.Id, groups);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{editedLog.Id} обновлена",
                    RepairLogId = editedLog.Id,
                    CommentatorId = editedLog.AuthorId,
                };

                await AddServerComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", model.Id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{model.Id}").SendAsync("UpdateLog", model.Id);

                _logger.LogInformation($"The {nameof(RepairLogDto)} editing was successful.");
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} editing failed.", ex);
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = "Ошибка изменения заявок" });
            }
        }

        // POST: RepairLog/ToExecute/5
        [HttpPost]
        [Authorize]
        public async Task ToExecute(int id)
        {
            try
            {
                await _repairLogService.ToExecute(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} принята к исполнению",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await AddServerComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/ToChecking/5
        [HttpPost]
        [Authorize]
        public async Task ToChecking(int id)
        {
            try
            {
                await _repairLogService.ToChecking(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} отправлена на проверку",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await AddServerComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/ToComplete/5
        [HttpPost]
        [Authorize]
        public async Task ToComplete(int id)
        {
            try
            {
                await _repairLogService.ToComplete(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} выполнена",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await AddServerComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/ToRevision/5
        [HttpPost]
        [Authorize]
        public async Task ToRevision(int id)
        {
            try
            {
                await _repairLogService.ToExecute(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} отправлена на доработку",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await AddServerComment(comment);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/ToRequest/5
        [HttpPost]
        [Authorize]
        public async Task ToArchive(int id)
        {
            try
            {
                await _repairLogService.ToArchive(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} отправлена в архив",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("ArchiveDeleteLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                await AddServerComment(comment);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/ToRequest/5
        [HttpPost]
        [Authorize]
        public async Task ToRequest(int id)
        {
            try
            {
                await _repairLogService.ToRequest(id);

                var comment = new CommentViewModel()
                {
                    Date = DateTime.Now,
                    Text = $"Заявка #{id} отправлена в запросы",
                    RepairLogId = id,
                    CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                };

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", id);
                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{id}").SendAsync("UpdateLog", id);

                await AddServerComment(comment);

                _logger.LogInformation($"The {nameof(id)} update was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} update failed.", ex);
            }
        }

        // POST: RepairLog/Appoint
        [HttpPost]
        public async Task Appoint(int logId, List<int> executors)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _repairLogService.AddUsersToLogExecutors(logId, executors);

            string commentText = $"Заявкe #{logId} изменены исполнители";

            var comment = new CommentViewModel()
            {
                Date = DateTime.Now,
                Text = commentText,
                RepairLogId = logId,
                CommentatorId = userId,
            };

            await AddServerComment(comment);

            await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", logId);
            await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{logId}").SendAsync("UpdateLog", logId);

            _logger.LogInformation($"The {nameof(logId)} appoint was successful.");
        }

        // GET: RepairLog/GetExecutorsComponent/5
        [Authorize]
        public IActionResult GetExecutorsComponent(int id)
        {
            return ViewComponent("Executors", new { logId = id, userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) });
        }

        // GET: RepairLog/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItem_RepairGroups_Comments_Author(id));
            return View(log);
        }

        // POST: RepairLog/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repairLogService.Delete(id);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("ArchiveDeleteLog", id);

                _logger.LogInformation($"The {nameof(RepairLogDto)} deleting was successful.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairLogDto)} deleting failed.", ex);
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = "Ошибка удаления заявок" });
            }
        }

        // POST: RepairLog/AddAutoComment
        [HttpPost]
        [Authorize]
        public async Task AddServerComment(CommentViewModel model)
        {
            try
            {
                var commentMap = _mapper.Map<CommentDto>(model);

                var id = await _repairLogService.AddCommentToLog(model.RepairLogId, commentMap);

                await _commentService.SendNewCommentToActiveUsers(id);

                await _repairLogDetailsHub.Clients.Group($"LogGroupDetails#{model.RepairLogId}").SendAsync("UpdateLog", model.RepairLogId);

                await AddLogNotification_ByRepairGroups(id, model.RepairLogId);

                _logger.LogInformation($"The {nameof(model)} commenting was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(CommentViewModel)} commenting failed.", ex);
            }
        }

        // POST: RepairLog/AddCustomComment
        [HttpPost]
        [Authorize]
        public async Task AddUserComment(CommentViewModel model)
        {
            try
            {
                model.Date = DateTime.Now;
                model.CommentatorId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var commentMap = _mapper.Map<CommentDto>(model);

                var id = await _repairLogService.AddCommentToLog(model.RepairLogId, commentMap);

                await _commentService.SendNewCommentToActiveUsers(id);

                await _repairLogIndexHub.Clients.Group("RepairLogIndex").SendAsync("UpdateLog", model.RepairLogId);

                _logger.LogInformation($"The {nameof(model)} commenting was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(CommentViewModel)} commenting failed.", ex);
            }
        }

        // POST: RepairLog/AddLogNotification_ByRepairGroups
        [HttpPost]
        [Authorize]
        public async Task AddLogNotification_ByRepairGroups(int commId, int logId)
        {
            try
            {
                var log = _mapper.Map<RepairLogViewModel>(await _repairLogService.GetItem_RepairGroups_Comments_Author(logId));
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
    }
}
