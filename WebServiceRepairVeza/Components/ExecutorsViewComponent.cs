using AutoMapper;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Components
{
    public class ExecutorsViewComponent : ViewComponent
    {
        private readonly RepairLogService _repairLogService;
        private readonly UserService _userService;

        private readonly IMapper _mapper;

        public ExecutorsViewComponent(IMapper mapper, RepairLogService repairLogService, UserService userService)
        {
            _mapper = mapper;
            _repairLogService = repairLogService;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int logId, int userId)
        {
            var log = await _repairLogService.GetItem_RepairGroups_Comments_Author(logId);
            var logGroupsId = log.RepairGroups?.Select(g => g.Id).ToList();
            
            if (logGroupsId != null)
            {
                var users = _mapper.Map<List<UserViewModel>>(await _userService.GetEmployersByRepairGroupsIds(logGroupsId));

                ChangeRepairLogExecutorsViewModel model = new()
                {
                    LogId = logId,
                    LogExecutors = _mapper.Map<IEnumerable<UserViewModel>>(log.Executors).ToList(),
                    AllExecutors = users.Where(us => us.Id != userId && us.Id != log.AuthorId).ToList()
                };

                return View("ExecutorsPartial", model);
            }

            return View("ExecutorsPartial", new ChangeRepairLogExecutorsViewModel());
        }
    }
}
