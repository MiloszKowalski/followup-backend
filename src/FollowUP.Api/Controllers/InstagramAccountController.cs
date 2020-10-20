using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    [Authorize]
    public class InstagramAccountController : ApiControllerBase
    {
        private readonly IInstagramAccountService _instagramAccountService;
        private readonly IAccountSettingsService _settingsService;

        public InstagramAccountController(IInstagramAccountService instagramAccountService,
            ICommandDispatcher commandDispatcher, IAccountSettingsService settingsService)
            : base(commandDispatcher)
        {
            _instagramAccountService = instagramAccountService;
            _settingsService = settingsService;
        }

        [Authorize(Policy = "admin")]
        [HttpGet("{page}/{pageSize}")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var accounts = await _instagramAccountService.GetAsync(page, pageSize);

            return Json(accounts);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUsersAccounts()
        {
            var accounts = await _instagramAccountService
                            .GetAllByUserIdExtendedAsync(Guid.Parse(User.Identity.Name));

            return Json(accounts);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            var accounts = await _instagramAccountService.GetAllByUserIdAsync(userId);

            return Json(accounts);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _instagramAccountService.GetCountAsync();

            return Json(count);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstagramAccount([FromBody]CreateInstagramAccount command)
        {
            await DispatchAsync(command);
            var accounts = await _instagramAccountService.GetAllByUserIdAsync(command.UserId);
            var account = accounts.FirstOrDefault(x => x.Username == command.Username);
            var accountInfo = await _instagramAccountService.GetExtendedInfoByIdAsync(account.Id);

            return Json(accountInfo);
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteInstagramAccount(Guid accountId)
        {
            var command = new DeleteInstagramAccount { InstagramAccountId = accountId };
            await DispatchAsync(command);
            var accounts = await _instagramAccountService.GetAllByUserIdAsync(command.UserId);

            return Json(accounts);
        }

        [HttpGet("{accountId}/settings")]
        public async Task<IActionResult> GetAccountsSettings(Guid accountId)
        {
            var settings = await _settingsService.GetAccountsSettingsAsync(accountId);

            return Json(settings);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateAccountSettings([FromBody]UpdateAccountSettings command)
        {
            await DispatchAsync(command);
            var settings = await _settingsService.GetAccountsSettingsAsync(command.InstagramAccountId);

            return Json(settings);
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
