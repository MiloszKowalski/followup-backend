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
        }

        public void SetNexMinId(string nextMinId)
        {
            if (string.IsNullOrWhiteSpace(nextMinId))
                return;

            NextMinId = nextMinId;
        }
    }
}
