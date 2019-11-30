using FollowUP.Domain;
using System;
using System.Text.RegularExpressions;

namespace FollowUP.Core.Domain
{
    public class ChildComment
    {
        private readonly string _guidRegex = @"^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$";

        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public long AuthorId { get; protected set; }
        public string Author { get; protected set; }
        public string ProfilePictureUri { get; protected set; }
        public string Content { get; protected set; }
        public int LikesCount { get; protected set; }
        public Guid ParentCommentId { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected ChildComment() { }

        public ChildComment(Guid accountId, long authorId, string author, string profilePictureUri, string content,
                        int likesCount, Guid parentCommentId, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = Guid.NewGuid();
            SetAccountId(accountId);
            AuthorId = authorId;
            Author = author;
            ProfilePictureUri = profilePictureUri;
            Content = content;
            SetLikesCount(likesCount);
            ParentCommentId = parentCommentId;
            CreatedAt = createdAt;
        }

        private void SetAccountId(Guid accountId)
        {
            if (Regex.IsMatch(accountId.ToString(), _guidRegex))
                throw new DomainException(ErrorCodes.InvalidGuid, $"Guid given to child comment is invalid.");

            AccountId = accountId;
        }

        private void SetLikesCount(int count)
        {
            if (count < 0)
                throw new DomainException(ErrorCodes.NegativeLikes, "Likes can't be negative");

            LikesCount = count;
        }
    }
}
