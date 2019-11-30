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
        public DateTimeOffset ActionCooldown { get; protected set; }
        public int PreviousCooldownMilliseconds { get; protected set; }

        protected Promotion() { }

        public Promotion(Guid promotionId, Guid accountId, PromotionType promotionType, string label, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = promotionId;
            AccountId = accountId;
            PromotionType = promotionType;
            Label = label;
            CreatedAt = createdAt;
            ActionCooldown = DateTime.UtcNow;
            PreviousCooldownMilliseconds = 0;
        }

        public void SetNexMinId(string nextMinId)
        {
            if (string.IsNullOrWhiteSpace(nextMinId))
                return;

            NextMinId = nextMinId;
        }

        public void SetActionCooldown(int milliseconds)
        {
            if (milliseconds < 0)
                return;

            var dateAfterCooldown = DateTime.UtcNow;
            dateAfterCooldown = dateAfterCooldown.AddMilliseconds(milliseconds);

            if (dateAfterCooldown < DateTime.UtcNow)
                return;

            ActionCooldown = dateAfterCooldown;
            PreviousCooldownMilliseconds = milliseconds;
        }
    }
}
