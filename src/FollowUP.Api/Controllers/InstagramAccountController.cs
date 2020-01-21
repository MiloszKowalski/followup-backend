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
    [Authorize]
    public class InstagramAccountController : ApiControllerBase
    {
        private readonly IInstagramAccountService _instagramAccountService;

        public InstagramAccountController(IInstagramAccountService instagramAccountService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _instagramAccountService = instagramAccountService;
        }

        [Authorize(Policy = "admin")]
        [HttpGet("{page}/{pageSize}")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var accounts = await _instagramAccountService.GetAsync(page, pageSize);

            return Json(accounts);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _instagramAccountService.GetCount();

            return Json(count);
        }

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

        [HttpPost("eblogin")]
        public async Task<IActionResult> LoginToEBInstagram([FromBody]LoginToEBInstagram command)
        {
            await DispatchAsync(command);
            var accounts = await _instagramAccountService.GetAllByUserId(command.UserId);
            var account = new AccountDto();
            foreach (var x in accounts)
            {
                if (x.Username == command.Username)
                    account = x;
            }

            return Json(account);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteInstagramAccount([FromBody]DeleteInstagramAccount command)
        {
            await DispatchAsync(command);
            var accounts = await _instagramAccountService.GetAllByUserId(command.UserId);

            return Json(accounts);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            var accounts = await _instagramAccountService.GetAllByUserId(userId);

            return Json(accounts);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> BuyCommentModule([FromBody]BuyComments command)
        {
            await DispatchAsync(command);

            return Ok();
        }

        [HttpPost("promotions")]
        public async Task<IActionResult> BuyPromotionsModule([FromBody]BuyPromotions command)
        {
            await DispatchAsync(command);

            return Ok();
        }
    }
}
