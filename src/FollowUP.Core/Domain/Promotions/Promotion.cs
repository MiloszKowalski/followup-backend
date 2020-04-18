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
        public DateTime NextMinIdDate { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected Promotion() { }

        public Promotion(Guid promotionId, Guid accountId, PromotionType promotionType, string label, DateTime createdAt)
        {
            Id = promotionId;
            SetAccountId(accountId);
            SetPromotionType(promotionType);
            SetLabel(label);
            CreatedAt = createdAt;
            NextMinIdDate = DateTime.UtcNow;
        }

        private void SetAccountId(Guid accountId)
        {
            if (accountId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given Account ID is null.");

            if (accountId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given Account ID is empty.");

            AccountId = accountId;
        }

        public void SetPromotionType(PromotionType promotionType)
        {
            if (promotionType < 0)
                throw new DomainException(ErrorCodes.NegativeEnum, "PromotionType cannot be negative.");

            if (string.IsNullOrEmpty(Enum.GetName(typeof(PromotionType), promotionType)))
                throw new DomainException(ErrorCodes.EnumOutOfBounds, "Given PromotionType doesn't exist.");

            if (promotionType == PromotionType)
                return;

            PromotionType = promotionType;
        }

        public void SetLabel(string label)
        {
            if (label == null)
                throw new DomainException(ErrorCodes.LabelIsNull, "Promotion's label is null!");

            if (string.IsNullOrWhiteSpace(label))
                throw new DomainException(ErrorCodes.LabelIsEmpty, "Promotion's label is empty!");

            if (label.Length > 100)
                throw new DomainException(ErrorCodes.LabelTooLong, "Promotion's label is too long!");

            if (label == Label)
                return;

            Label = label;
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

        public static Promotion GetUnfollowPromotion(Guid accountId)
        {
            return new Promotion(Guid.NewGuid(), accountId, PromotionType.Unfollow, "", DateTime.Now);
        }
    }
}
