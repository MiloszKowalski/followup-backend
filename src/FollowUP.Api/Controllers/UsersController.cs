﻿using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    [Authorize]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenManager _tokenManager;

        public UsersController(IUserService userService, ITokenManager tokenManager,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _userService = userService;
            _tokenManager = tokenManager;
        }
        
        [Authorize(Policy = "admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.BrowseAsync();

            return Json(users);
        }

        [Authorize(Policy = "admin")]
        [HttpGet("{guid}")]
        public async Task<IActionResult> Get(Guid guid)
        {
            var user = await _userService.GetAsync(guid);
            if (user == null)
            {
                return NotFound();
            }

            return Json(user);
        }

        [AllowAnonymous]
        [HttpPost("tokens/{token}/refresh")]
        public async Task<IActionResult> RefreshAccessToken(string token)
            => Ok(await _userService.RefreshAccessTokenAsync(token));

        [AllowAnonymous]
        [HttpPost("tokens/{token}/revoke")]
        public async Task<IActionResult> RevokeRefreshToken(string token)
        {
            await _userService.RevokeRefreshTokenAsync(token);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("tokens/cancel")]
        public async Task<IActionResult> CancelAccessToken()
        {
            await _tokenManager.DeactivateCurrentAsync();

            return NoContent();
        }
    }
}
