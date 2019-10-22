using System;

namespace FollowUP.Core.Domain
{
    public class CompletedMedia
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public string Code { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected CompletedMedia() { }

        public CompletedMedia(Guid promotionId, Guid accountId, string code, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = promotionId;
            AccountId = accountId;
            Code = code;
            CreatedAt = createdAt;
        }
    }
}
