using System;

namespace FollowUP.Core.Domain
{
    public class PromotionComment
    {
        public Guid Id { get; protected set; }
        public string Content { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        protected PromotionComment() { }

        public PromotionComment(Guid promotionId, Guid accountId, string content, DateTime createdAt)
        {
            Id = promotionId;
            SetAccountId(accountId);
            SetContent(content);
            CreatedAt = createdAt;
        }

        private void SetAccountId(Guid accountId)
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

        public void SetContent(string content)
        {
            if (Content == content)
            {
                return;
            }

            if (content == null)
            {
                throw new DomainException(ErrorCodes.ContentIsNull,
                    "Comment's content is null!");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new DomainException(ErrorCodes.ContentIsEmpty,
                    "Comment's content is empty!");
            }

            if (content.Length > 512)
            {
                throw new DomainException(ErrorCodes.ContentTooLong,
                    "Comment's content is too long!");
            }

            Content = content;
        }
    }
}
