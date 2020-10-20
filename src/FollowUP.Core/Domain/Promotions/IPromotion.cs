using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public interface IPromotion
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }

        Guid InstagramAccountId { get; set; }
        InstagramAccount InstagramAccount { get; set; }
        IList<DailyPromotionPercentage> DailyPromotionPercentages { get; set; }
    }
}
