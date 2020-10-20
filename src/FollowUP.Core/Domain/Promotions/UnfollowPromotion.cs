using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public class UnfollowPromotion : IPromotion
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }
        public IList<DailyPromotionPercentage> DailyPromotionPercentages { get; set; }

        public UnfollowPromotion(Guid id, Guid instagramAccountId, DateTime createdAt)
        {
            Id = id;
            SetInstagramAccountId(instagramAccountId);
            CreatedAt = createdAt;
        }

        private void SetInstagramAccountId(Guid accountId)
        {
            if (InstagramAccountId == accountId)
            {
                return;
            }

            if (accountId == null)
            {
                throw new DomainException(ErrorCodes.GuidIsNull,
                    "The given Account ID is null.");
            }

            if (accountId == Guid.Empty)
            {
                throw new DomainException(ErrorCodes.GuidIsEmpty,
                    "The given Account ID is empty.");
            }

            InstagramAccountId = accountId;
        }
    }
}
