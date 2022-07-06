using AutoMapper;
using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebService.Models.ViewModels.Account;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public AccountApiController(SignInManager<User> signInManager, UserService userService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userService = userService;
            _mapper = mapper;
        }

        // POST api/AccountApi
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result =
                        await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var user = _mapper.Map<UserViewModel>(await _userService.GetItem(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)));
                        return Ok(user);
                    }
                    else
                    {
                        return Forbid();
                    }
                }

                return BadRequest();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}