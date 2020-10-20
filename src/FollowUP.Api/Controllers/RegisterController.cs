using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Accounts;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    public class RegisterController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(ICommandDispatcher commandDispatcher, IUserService userService)
            : base(commandDispatcher)
        {
            _userService = userService;
        }

        [HttpGet("{userId}/{registrationToken}")]
        public async Task<IActionResult> Get(Guid userId, string registrationToken)
        {
            await _userService.ConfirmEmailTokenAsync(userId, registrationToken);

            var user = await _userService.GetAsync(userId);

            if(user.Verified)
            {
                return Created($"users/{userId}", null);
            }    

            return StatusCode(500);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Register command)
        {
            await DispatchAsync(command);

            return Created($"users/{command.Email}", null);
        }
    }
}
