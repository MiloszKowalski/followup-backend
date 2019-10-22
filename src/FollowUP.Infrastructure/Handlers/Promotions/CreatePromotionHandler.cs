using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class CreatePromotionHandler : ICommandHandler<CreatePromotion>
    {
        private readonly IPromotionService _promotionService;

        public CreatePromotionHandler(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public async Task HandleAsync(CreatePromotion command)
        {
            await _promotionService.CreatePromotion(command.AccountId, command.PromotionType, command.Label);
        }
    }
}
