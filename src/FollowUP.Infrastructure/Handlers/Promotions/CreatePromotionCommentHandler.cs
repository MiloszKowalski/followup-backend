using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class CreatePromotionCommentHandler : ICommandHandler<CreatePromotionComment>
    {
        private readonly IPromotionService _promotionService;

        public CreatePromotionCommentHandler(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public async Task HandleAsync(CreatePromotionComment command)
        {
            await _promotionService.CreatePromotionComment(command.AccountId, command.Content);
        }
    }
}
