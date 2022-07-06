﻿using AutoMapper;
using BLL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebService.Models.ViewModels.User;

namespace WebService.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        readonly IMapper _mapper;
        readonly UserService _userService;

        public UsersApiController(IMapper mapper, UserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            try
            {
                var roles = await _userService.GetUserRoles(id);

                if (roles.Count > 0)
                    return Ok(roles);
                else return NoContent();
            }
            catch { return BadRequest(); }
        }
    }
}