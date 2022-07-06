using AutoMapper;
using BLL.Models;
using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Models.ViewModels.RepairGroup;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers
{
    public class RepairGroupController : Controller
    {
        private readonly RepairGroupService _repairGroupService;

        private readonly ILogger<RepairGroupController> _logger;
        private readonly IMapper _mapper;
        readonly UserService _userService;

        public RepairGroupController(ILogger<RepairGroupController> logger, IMapper mapper, RepairGroupService repairGroupService, UserService userService)
        {
            _repairGroupService = repairGroupService;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> UserRepairGroups() => View(_mapper.Map<IEnumerable<UserViewModel>>(await _userService.GetItems()));

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var groups = await _repairGroupService.GetItems();
            return View(_mapper.Map<IEnumerable<RepairGroupViewModel>>(groups));
        }

        // GET: RepairGroup/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: RepairGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(RepairGroupViewModel repairGroup)
        {
            if (!ModelState.IsValid)
            {
                return View(repairGroup);
            }
            try
            {
                var groupMap = _mapper.Map<RepairGroupDto>(repairGroup);
                await _repairGroupService.Create(groupMap);
                _logger.LogInformation($"The {nameof(RepairGroupDto)} creation was successful.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairGroupDto)} creation failed.", ex);
                return View();
            }
        }

        // GET: RepairGroup/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var group = _mapper.Map<RepairGroupViewModel>(await _repairGroupService.GetItem(id));
            return View(group);
        }

        // POST: RepairGroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(RepairGroupViewModel groupViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(groupViewModel);
            }
            try
            {
                var groupMap = _mapper.Map<RepairGroupDto>(groupViewModel);
                await _repairGroupService.Update(groupMap);
                _logger.LogInformation($"The {nameof(RepairGroupDto)} editing was successful.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairGroupDto)} editing failed.", ex);
                return View();
            }
        }

        // POST: RepairGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repairGroupService.Delete(id);
                _logger.LogInformation($"The {nameof(RepairGroupDto)} deleting was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairGroupDto)} deleting failed.", ex);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeRepairGroup(int userId)
        {
            var user = _mapper.Map<User>(await _userService.GetItem(userId));
            // получем список групп пользователя
            var userGroups = _mapper.Map<IEnumerable<RepairGroupViewModel>>(await _repairGroupService.GetUserGroups(userId));

            if (user != null && userGroups != null)
            {
                var groups = _mapper.Map<List<RepairGroupViewModel>>(await _repairGroupService.GetItems());

                ChangeRepairGroupViewModel model = new()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserGroups = userGroups.ToList(),
                    AllGroups = groups
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeRepairGroup(int userId, List<string> groups)
        {
            try
            {
                if (groups != null)
                {
                    await _userService.EditGroupsIntoUserAsync(userId, groups);

                    _logger.LogInformation($"The {nameof(RepairGroupDto)} changing repair group was successful.");
                    return RedirectToAction("Index", "Users");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(RepairGroupDto)} changing repair group failed.", ex);
            }

            return NotFound();
        }
    }
}
