using AutoMapper;
using BLL.Models;
using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers.IdentityControllers
{
    public class UsersController : Controller
    {
        readonly IMapper _mapper;
        readonly UserService _userService;

        public UsersController(IMapper mapper, UserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync() => View(_mapper.Map<IEnumerable<UserViewModel>>(await _userService.GetUsersDetails()));

        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserViewModel()
                {
                    Name = model.Name,
                    SurName = model.SurName,
                    MiddleName = model.MiddleName,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                };

                await _userService.Create(_mapper.Map<UserDto>(user), model.Password);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == id || User.IsInRole("admin"))
            {
                var user = _mapper.Map<UserViewModel>(await _userService.GetItem(id));
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            return RedirectToAction("UserProfile", new { id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if ((userId == model.Id || User.IsInRole("admin")) && ModelState.IsValid)
                {
                    IdentityResult result = await _userService.UpdateUser(_mapper.Map<UserDto>(model));
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserProfile", new { id = model.Id });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return RedirectToAction("UserProfile", new { id = model.Id });
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);

            return RedirectToAction("IndexAsync");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var user = _mapper.Map<UserViewModel>(await _userService.GetItem(id));
            if (user == null)
            {
                return NotFound();
            }

            ChangePasswordViewModel change = new()
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return View(change);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<UserViewModel>(await _userService.GetItem(model.Id));
                if (user != null)
                {
                    if (model.NewPassword != null)
                    {
                        IdentityResult result = await _userService.SetNewPasswordAsync(user.UserName, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("UserProfile", new { id = model.Id });
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangePersonalPassword(int id)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == id || User.IsInRole("admin"))
            {
                var user = _mapper.Map<UserViewModel>(await _userService.GetItem(id));
                if (user == null)
                {
                    return NotFound();
                }

                ChangePasswordViewModel change = new()
                {
                    Id = user.Id,
                    UserName = user.UserName
                };

                return View(change);
            }
            return RedirectToAction("UserProfile", new { id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePersonalPassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if ((userId == model.Id || User.IsInRole("admin")) && ModelState.IsValid)
                {
                    var user = _mapper.Map<UserViewModel>(await _userService.GetItem(model.Id));
                    if (user != null)
                    {
                        if (model.OldPassword != null && model.NewPassword != null)
                        {
                            IdentityResult result = await _userService.ChangePasswordAsync(user.UserName, model.OldPassword, model.NewPassword);
                            if (result.Succeeded)
                            {
                                return RedirectToAction("UserProfile", new { id = model.Id });
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    }
                }
                return View(model);
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserProfile(int id)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            UserViewModel user;

            if (userId == id)
            {
                ViewData["myProfile"] = true;
                user = _mapper.Map<UserViewModel>(await _userService.GetUserDetails(id));
            }
            else
            {
                if (id == 0)
                {
                    ViewData["myProfile"] = true;
                    user = _mapper.Map<UserViewModel>(await _userService.GetUserDetails(userId));
                }
                else
                {
                    ViewData["myProfile"] = false;
                    user = _mapper.Map<UserViewModel>(await _userService.GetUserDetails(id));
                }
            }

            return View(new UserChart(user!, GetChartDoughnutOfUser(user), GetChartChartBarOfUser(user)));
        }

        public Chart GetChartDoughnutOfUser(UserViewModel user)
        {
            List<double> data = new();
            List<string> backgrounds = new();
            List<string> lables = new();
            if (user.LogExecutors != null && user.LogExecutors.Any())
            {
                if (user.LogExecutors.Any(_ => _.Status == RepairStatus.AtWork))
                {
                    data.Add(user.LogExecutors.Count(_ => _.Status == RepairStatus.AtWork));
                    backgrounds.Add("#0d6efd");
                    lables.Add("В работе");
                }
                if (user.LogExecutors.Any(_ => _.Status == RepairStatus.Check))
                {
                    data.Add(user.LogExecutors.Count(_ => _.Status == RepairStatus.Check));
                    backgrounds.Add("#ffc107");
                    lables.Add("На проверке");
                }
                if (user.LogExecutors.Any(_ => _.Status == RepairStatus.Completed || _.Status == RepairStatus.Archive))
                {
                    data.Add(user.LogExecutors.Count(_ => _.Status == RepairStatus.Completed || _.Status == RepairStatus.Archive));
                    backgrounds.Add("#198754");
                    lables.Add("Выполнено");
                }
            }

            Chart chart = new()
            {
                Data = data.ToArray(),
                Backgrounds = backgrounds.ToArray(),
                Lables = lables.ToArray()
            };

            return chart;
        }

        public Chart GetChartChartBarOfUser(UserViewModel user)
        {
            List<double> data = new();
            List<string> backgrounds = new();
            List<string> lables = new();
            if (user.LogExecutors != null && user.LogExecutors.Any())
            {
                var comletedLogs = user.LogExecutors.Where(_ => _.Status == RepairStatus.Archive || _.Status == RepairStatus.Completed);

                if (comletedLogs.Any())
                {
                    var logs = comletedLogs.Where(_ => _.ChangedDate.Year == DateTime.Now.Year);

                    for (int month = 1; month <= DateTime.Now.Month; month++)
                    {
                        data.Add(logs.Count(_ => _.ChangedDate.Month == month));
                        backgrounds.Add("#0dcaf0");
                        lables.Add(new DateTime(0001, month, 1).ToString("MMMM"));
                    }
                }
            }

            Chart chart = new()
            {
                Data = data.ToArray(),
                Backgrounds = backgrounds.ToArray(),
                Lables = lables.ToArray()
            };

            return chart;
        }

        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == id)
                ViewData["myProfile"] = true;
            else ViewData["myProfile"] = false;

            var user = _mapper.Map<UserViewModel>(await _userService.GetUserDetails(id));

            return PartialView($"../Users/ModalUserProfile", user);
        }
    }
}