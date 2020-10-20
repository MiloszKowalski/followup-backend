using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public abstract class FollowPromotion : IPromotion
    {
        public Guid Id { get; set; }

        public string Label { get; protected set; }
        public string LastMediaId { get; protected set; }
        public string NextMinId { get; protected set; }

        public DateTime NextMinIdDate { get; protected set; }
        public DateTime CreatedAt { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }
        public IList<DailyPromotionPercentage> DailyPromotionPercentages { get; set; }

        public FollowPromotion(Guid id, Guid instagramAccountId,
            string label, DateTime createdAt)
        {
            Id = id;
            SetInstagramAccountId(instagramAccountId);
            SetLabel(label);
            CreatedAt = createdAt;
            NextMinIdDate = DateTime.UtcNow;
        }

        protected virtual void SetInstagramAccountId(Guid accountId)
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

        protected virtual void SetLabel(string label)
        {
            if (Label == label)
            {
                return;
            }

            if (label == null)
            {
                throw new DomainException(ErrorCodes.LabelIsNull,
                    "Promotion's label is null!");
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new DomainException(ErrorCodes.LabelIsEmpty,
                    "Promotion's label is empty!");
            }

            if (label.Length > 100)
            {
                throw new DomainException(ErrorCodes.LabelTooLong,
                    "Promotion's label is too long!");
            }

            Label = label;
        }

        public virtual void SetNextMinId(string nextMinId)
        {
            if (string.IsNullOrWhiteSpace(nextMinId))
            {
                return;
            }

            NextMinId = nextMinId;
        }

        public virtual void SetNextMinIdDate(DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                return;
            }

            NextMinIdDate = date;
        }

        public virtual void SetLastMediaId(string lastMediaId)
        {
            if (string.IsNullOrWhiteSpace(lastMediaId))
            {
                return;
            }

            LastMediaId = lastMediaId;
        }
    }
}
