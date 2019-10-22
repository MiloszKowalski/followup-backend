using System;

namespace FollowUP.Core.Domain
{
    public class ChildComment
    {
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
            AccountId = accountId;
            AuthorId = authorId;
            Author = author;
            ProfilePictureUri = profilePictureUri;
            Content = content;
            LikesCount = likesCount;
            ParentCommentId = parentCommentId;
            CreatedAt = createdAt;
        }
    }
}
