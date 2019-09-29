using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public class Comment
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public long AuthorId { get; protected set; }
        public string Author { get; protected set; }
        public string ProfilePictureUri { get; protected set; }
        public string Content { get; protected set; }
        public int LikesCount { get; protected set; }
        public string ParentMediaId { get; protected set; }
        public string ParentImageUri { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected Comment() { }

        public Comment(Guid commentId, Guid accountId, long authorId, string author,
                        string profilePictureUri, string content, int likesCount,
                        string parentMediaId, string parentImageUri, DateTimeOffset createdAt)
        {
            // TODO: Sanitize the fields
            Id = commentId;
            AccountId = accountId;
            AuthorId = authorId;
            Author = author;
            ProfilePictureUri = profilePictureUri;
            Content = content;
            LikesCount = likesCount;
            ParentMediaId = parentMediaId;
            ParentImageUri = parentImageUri;
            CreatedAt = createdAt;
        }

        public Comment NewComment(Guid commentId, Guid accountId, long authorId, string author, string profilePictureUri, string content,
                                  int likesCount, string parentMediaId, string parentImageUri, DateTimeOffset createdAt)
            => new Comment(commentId, accountId, authorId, author, profilePictureUri, content,
                           likesCount, parentMediaId, parentImageUri, createdAt);
    }
}
