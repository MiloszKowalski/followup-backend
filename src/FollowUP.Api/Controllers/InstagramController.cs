using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    public class InstagramController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IInstagramAccountService _instagramAccountService;

        public InstagramController(IUserService userService, IInstagramAccountService instagramAccountService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _userService = userService;
            _instagramAccountService = instagramAccountService;
        }

        public async Task<IActionResult> Get()
        {
            var users = await _userService.BrowseAsync();

            return Json(users);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateInstagramAccount command)
        {
            await DispatchAsync(command);

            return Created($"accounts/{command.Username}", null);
        }

        [Authorize]
        [HttpPost("login")]
        public async Task<IActionResult> LoginToInstagram([FromBody]LoginToInstagram command)
        {
            await DispatchAsync(command);
            var accounts = await _instagramAccountService.GetAllByUserId(command.UserId);
            var account = new AccountDto();
            foreach(var x in accounts)
            {
                if (x.Username == command.Username)
                    account = x;
            }

            return Json(account);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            var accounts = await _instagramAccountService.GetAllByUserId(userId);

            return Json(accounts);
        }
    }
}
