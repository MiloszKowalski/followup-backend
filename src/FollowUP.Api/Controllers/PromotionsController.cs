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
            var promotions = await _promotionService.GetAllPromotionsByAccountIdAsync(accountId);

            return Json(promotions);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody]CreatePromotion command)
        {
            await DispatchAsync(command);
            var promotions = await _promotionService.GetAllPromotionsByAccountIdAsync(command.AccountId);
            var promotion = promotions.SingleOrDefault
            (
                x => x.Label == command.Label && x.PromotionType == command.PromotionType
            );

            return Created($"{command.AccountId}", promotion);
        }

        [HttpDelete("{promotionId}")]
        public async Task<IActionResult> DeletePromotion(Guid promotionId)
        {
            var command = new DeletePromotion { PromotionId = promotionId };
            await DispatchAsync(command);

            return NoContent();
        }

        [HttpGet("comments/{accountId}")]
        public async Task<IActionResult> GetAccountsPromotionComments(Guid accountId)
        {
            var comments = await _promotionService.GetAllPromotionCommentsByAccountIdAsync(accountId);

            return Json(comments);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreatePromotionComment([FromBody]CreatePromotionComment command)
        {
            await DispatchAsync(command);

            return Created($"{command.AccountId}", null); ;
        }
    }
}
