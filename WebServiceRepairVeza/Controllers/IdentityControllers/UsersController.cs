using AutoMapper;
using BLL.Models;
using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebService.Models.ViewModels;
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

        [Authorize]
        public async Task<IActionResult> AccountProfile()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return View(_mapper.Map<UserViewModel>(await _userService.GetItem(userId)));
        }

        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            return PartialView("../Users/DetailsPartial", _mapper.Map<UserViewModel>(await _userService.GetUserDetails(id)));
        }

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
            var user = _mapper.Map<UserViewModel>(await _userService.GetItem(id));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _userService.UpdateUser(_mapper.Map<UserDto>(model));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return RedirectToAction("AccountProfile");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePersonalInfo(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _userService.UpdateUser(_mapper.Map<UserDto>(model));
                if (result.Succeeded)
                {
                    return RedirectToAction("AccountProfile");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return RedirectToAction("AccountProfile");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);

            return RedirectToAction("Index");
        }

        [Authorize]
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
                            return RedirectToAction("Index");
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
            return RedirectToAction("AccountProfile");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePersonalPassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<UserViewModel>(await _userService.GetItem(model.Id));
                if (user != null)
                {
                    if (model.OldPassword != null && model.NewPassword != null)
                    {
                        IdentityResult result = await _userService.ChangePasswordAsync(user.UserName, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("AccountProfile");
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
            return RedirectToAction("AccountProfile");
        }
    }
}