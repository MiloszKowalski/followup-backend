using FollowUP.Core.Domain;
using System;

namespace FollowUP.Infrastructure.Factories
{
    public static class PromotionFactory
    {
        public static UnfollowPromotion GetUnfollowPromotion(Guid accountId)
        {
            return new UnfollowPromotion(Guid.NewGuid(), accountId, DateTime.Now);
        }
    }
}
