using FollowUP.Controllers;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    [Authorize]
    public class PromotionsController : ApiControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IInstagramApiService _apiService;
        private readonly IInstagramAccountRepository _accountRepository;

        public PromotionsController(IPromotionService promotionService, IInstagramApiService apiService,
            IInstagramAccountRepository accountRepository,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _promotionService = promotionService;
            _apiService = apiService;
            _accountRepository = accountRepository;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId)
        {
            var promotionComments = await _promotionService.GetAllPromotionCommentsByAccountId(accountId);

            return Json(promotionComments);
        }

        [HttpPost("{accountId}/unfollow/{count}")]
        public async Task<IActionResult> UnfollowUsers(Guid accountId, int count)
        {
            var account = await _accountRepository.GetAsync(accountId);
            var instaApi = await _apiService.GetInstaApi(account);
            await _apiService.SendColdStartMockupRequests(instaApi, account);
            await Task.Delay(2137);
            await _apiService.GetUserProfileMockAsync(instaApi, account);
            await Task.Delay(1428);
            await _apiService.GetUserFollowedAsync(instaApi, account);
            await Task.Delay(4000);
            await _apiService.UnfollowUsersAsync(instaApi, account, count);


            return Json("Jej kurwa");
        }

        [HttpPost("{accountId}/tag/{tag}/{count}")]
        public async Task<IActionResult> UnfollowUsers(Guid accountId, string tag, int count)
        {
            var account = await _accountRepository.GetAsync(accountId);
            var instaApi = await _apiService.GetInstaApi(account);
            await _apiService.SendColdStartMockupRequests(instaApi, account);
            await Task.Delay(2137);
            await _apiService.GetHashtagMediaAsync(instaApi, account, tag);
            await Task.Delay(3412);
            await _apiService.LikeHashtagMediaAsync(instaApi, account, tag, count);

            return Json("Jej kurwa xd");
        }

        [HttpGet("comments/{accountId}")]
        public async Task<IActionResult> GetAccountsPromotionComments(Guid accountId)
        {
            var comments = await _promotionService.GetAllPromotionCommentsByAccountId(accountId);

            return Json(comments);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody]CreatePromotion command)
        {
            await DispatchAsync(command);

            var promotions = await _promotionService.GetAllPromotionsByAccountId(command.AccountId);
            return Created($"{command.AccountId}", promotions); ;
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreatePromotionComment([FromBody]CreatePromotionComment command)
        {
            await DispatchAsync(command);

            return Created($"{command.AccountId}", null); ;
        }
    }
}
