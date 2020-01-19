using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Get()
        {
            var users = await _userService.BrowseAsync();

            return Json(users);
        }

        [Authorize]
        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _userService.GetAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Json(user);
        }

        [HttpPost("tokens/{token}/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string token)
        => Ok(await _userService.RefreshAccessToken(token));

        [HttpPost("tokens/{token}/revoke")]
        public async Task<IActionResult> RevokeRefreshToken(string token)
        {
            await _userService.RevokeRefreshToken(token);

            return NoContent();
        }
    }
}
