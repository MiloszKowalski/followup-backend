using System;

namespace FollowUP.Infrastructure.Commands
{
    public class DeletePromotion : AuthenticatedCommandBase
    {
        public Guid PromotionId { get; set; }
    }
}
