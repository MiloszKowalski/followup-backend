using System;

namespace FollowUP.Core.Domain
{
    public class Promotion
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public PromotionType PromotionType { get; protected set; }
        public string Label { get; protected set; }
        public string NextMinId { get; protected set; }
        public string LastMediaId { get; protected set; }
        public DateTimeOffset NextMinIdDate { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected Promotion() { }

        public Promotion(Guid promotionId, Guid accountId, PromotionType promotionType, string label, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = promotionId;
            AccountId = accountId;
            PromotionType = promotionType;
            Label = label;
            CreatedAt = createdAt;
            NextMinIdDate = DateTime.UtcNow;
        }

        public void SetNextMinId(string nextMinId)
        {
            if (string.IsNullOrWhiteSpace(nextMinId))
                return;

            NextMinId = nextMinId;
        }

        public void SetNextMinIdDate(DateTime date)
        {
            if (date > DateTime.UtcNow)
                return;

            NextMinIdDate = date;
        }

        public void SetLastMediaId(string lastMediaId)
        {
            if (string.IsNullOrWhiteSpace(lastMediaId))
                return;

            LastMediaId = lastMediaId;
        }

    }
}
