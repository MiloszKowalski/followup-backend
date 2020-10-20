using System;

namespace FollowUP.Core.Domain
{
    public class ChildComment
    {
        public Guid Id { get; protected set; }

        public string Author { get; protected set; }
        public string AuthorPk { get; protected set; }
        public string Content { get; protected set; }
        public string ProfilePictureUri { get; protected set; }

        public int LikesCount { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public Guid CommentId { get; protected set; }
        public Comment Comment { get;set; }

        protected ChildComment() { }

        public ChildComment(Guid parentCommentId, string authorPk, string author, string profilePictureUri,
            string content, int likesCount, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            SetParentCommentId(parentCommentId);
            SetAuthorPk(authorPk);
            SetAuthor(author);
            SetProfilePictureUri(profilePictureUri);
            SetContent(content);
            SetLikesCount(likesCount);
            CreatedAt = createdAt;
        }
        private void SetParentCommentId(Guid parentCommentId)
        {
            if (CommentId == parentCommentId)
            {
                return;
            }

            if (parentCommentId == null)
            {
                throw new DomainException(ErrorCodes.GuidIsNull,
                    "The given parent Comment ID is null.");
            }

            if (parentCommentId == Guid.Empty)
            {
                throw new DomainException(ErrorCodes.GuidIsEmpty,
                    "The given parent Comment ID is empty.");
            }

            CommentId = parentCommentId;
        }

        private void SetAuthorPk(string authorPk)
        {
            if (AuthorPk == authorPk)
            {
                return;
            }

            if (authorPk.Length < 0)
            {
                throw new DomainException(ErrorCodes.InvalidAuthorId,
                    "Child comment's author's ID has to be a positive number.");
            }

            if (authorPk.Length > 15)
            {
                throw new DomainException(ErrorCodes.InvalidAuthorId,
                    "Child comment's author's ID is longer than Instagram's max Pk length!");
            }

            AuthorPk = authorPk;
        }

        private void SetAuthor(string author)
        {
            if (Author == author)
            {
                return;
            }

            if (author == null)
            {
                throw new DomainException(ErrorCodes.AuthorIsNull,
                    "Child comment's author's name is null!");
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new DomainException(ErrorCodes.AuthorIsEmpty,
                    "Child comment's author's name is empty!");
            }

            if (author.Length > 128)
            {
                throw new DomainException(ErrorCodes.AuthorTooLong,
                    "Child comment's author's name is too long!");
            }

            Author = author;
        }

        private void SetProfilePictureUri(string profilePictureUri)
        {
            if (ProfilePictureUri == profilePictureUri)
            {
                return;
            }

            if (profilePictureUri == null)
            {
                throw new DomainException(ErrorCodes.ProfilePictureUriIsNull,
                    "Profile's picture uri is null!");
            }

            if (string.IsNullOrWhiteSpace(profilePictureUri))
            {
                throw new DomainException(ErrorCodes.ProfilePictureUriIsEmpty,
                    "Profile's picture uri is empty!");
            }

            if (profilePictureUri.Length > 512)
            {
                throw new DomainException(ErrorCodes.AuthorTooLong,
                    "Profile's picture uri is too long!");
            }

            ProfilePictureUri = profilePictureUri;
        }

        private void SetContent(string content)
        {
            if (Content == content)
            {
                return;
            }

            if (content == null)
            {
                throw new DomainException(ErrorCodes.ContentIsNull,
                    "Child comment's content is null!");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new DomainException(ErrorCodes.ContentIsEmpty,
                    "Child comment's content is empty!");
            }

            if (content.Length > 512)
            {
                throw new DomainException(ErrorCodes.ContentTooLong,
                    "Child comment's content is too long!");
            }

            Content = content;
        }


        private void SetLikesCount(int count)
        {
            if (count < 0)
            {
                throw new DomainException(ErrorCodes.NegativeLikes,
                    "Child comment's likes can't be negative");
            }

            LikesCount = count;
        }
    }
}
