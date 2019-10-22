using System;

namespace FollowUP.Core.Domain
{
    public class PromotionComment
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public string Content { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected PromotionComment() { }

        public PromotionComment(Guid promotionId, Guid accountId, string content, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = promotionId;
            AccountId = accountId;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}
