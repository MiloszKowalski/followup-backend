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
        public DateTime CreatedAt { get; protected set; }

        protected ChildComment() { }

        public ChildComment(Guid accountId, long authorId, string author, string profilePictureUri, string content,
                        int likesCount, Guid parentCommentId, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            SetAccountId(accountId);
            SetAuthorId(authorId);
            SetAuthor(author);
            SetProfilePictureUri(profilePictureUri);
            SetContent(content);
            SetLikesCount(likesCount);
            SetParentCommentId(parentCommentId);
            CreatedAt = createdAt;
        }

        private void SetAccountId(Guid accountId)
        {
            if (accountId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given Account ID is null.");

            if (accountId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given Account ID is empty.");

            AccountId = accountId;
        }

        private void SetAuthorId(long authorId)
        {
            if (authorId < 0)
                throw new DomainException(ErrorCodes.InvalidAuthorId, "Child comment's author's ID has to be a positive long number.");

            if (authorId > long.MaxValue)
                throw new DomainException(ErrorCodes.InvalidAuthorId, "Child comment's author's ID is bigger than long's max value!");

            AuthorId = authorId;
        }

        private void SetAuthor(string author)
        {
            if (author == null)
                throw new DomainException(ErrorCodes.AuthorIsNull, "Child comment's author's name is null!");

            if (string.IsNullOrWhiteSpace(author))
                throw new DomainException(ErrorCodes.AuthorIsEmpty, "Child comment's author's name is empty!");

            if (author.Length > 128)
                throw new DomainException(ErrorCodes.AuthorTooLong, "Child comment's author's name is too long!");

            if (author == Author)
                return;

            Author = author;
        }

        private void SetProfilePictureUri(string profilePictureUri)
        {
            if (profilePictureUri == null)
                throw new DomainException(ErrorCodes.ProfilePictureUriIsNull, "Profile's picture uri is null!");

            if (string.IsNullOrWhiteSpace(profilePictureUri))
                throw new DomainException(ErrorCodes.ProfilePictureUriIsEmpty, "Profile's picture uri is empty!");

            if (profilePictureUri.Length > 512)
                throw new DomainException(ErrorCodes.AuthorTooLong, "Profile's picture uri is too long!");

            if (profilePictureUri == ProfilePictureUri)
                return;

            ProfilePictureUri = profilePictureUri;
        }

        private void SetContent(string content)
        {
            if (content == null)
                throw new DomainException(ErrorCodes.ContentIsNull, "Child comment's content is null!");

            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException(ErrorCodes.ContentIsEmpty, "Child comment's content is empty!");

            if (content.Length > 512)
                throw new DomainException(ErrorCodes.ContentTooLong, "Child comment's content is too long!");

            if (content == Content)
                return;

            Content = content;
        }

        private void SetParentCommentId(Guid parentCommentId)
        {
            if (parentCommentId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given ParentComment ID is null.");

            if (parentCommentId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given ParentComment ID is empty.");

            if (parentCommentId == ParentCommentId)
                return;

            ParentCommentId = parentCommentId;
        }

        private void SetLikesCount(int count)
        {
            if (count < 0)
                throw new DomainException(ErrorCodes.NegativeLikes, "Child comment's likes can't be negative");

            LikesCount = count;
        }
    }
}
