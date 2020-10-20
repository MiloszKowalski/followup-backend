using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Services;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class DeletePromotionHandler : ICommandHandler<DeletePromotion>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionService _promotionService;

        public DeletePromotionHandler(IPromotionRepository promotionRepository,
            IPromotionService promotionService)
        {
            _promotionRepository = promotionRepository;
            _promotionService = promotionService;
        }

        public async Task HandleAsync(DeletePromotion command)
        {
            var promotion = await _promotionRepository.GetAsync(command.PromotionId);

            if (promotion == null)
            {
                throw new ServiceException(ErrorCodes.PromotionNotFound,
                    $"Promotion with ID: {command.PromotionId} doesn't exist.");
            }

            if (promotion.InstagramAccount.User.Id != command.UserId)
            {
                throw new ServiceException(ErrorCodes.UserNotPermitted,
                    $"Promotion with ID: {command.PromotionId} doesn't belong to any " +
                    $"account that the user with ID: {command.UserId} owns.");
            }

            await _promotionService.DeletePromotionAsync(command.PromotionId);
        }
    }
}
