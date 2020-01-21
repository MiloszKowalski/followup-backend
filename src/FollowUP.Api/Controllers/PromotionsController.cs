using FollowUP.Controllers;
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

        public PromotionsController(IPromotionService promotionService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _promotionService = promotionService;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId)
        {
            var promotionComments = await _promotionService.GetAllPromotionCommentsByAccountId(accountId);

            return Json(promotionComments);
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
