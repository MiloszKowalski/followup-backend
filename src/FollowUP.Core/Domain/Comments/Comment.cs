using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public class Comment
    {
        public Guid Id { get; protected set; }

        public string Author { get; protected set; }
        public string AuthorPk { get; protected set; }
        public string Content { get; protected set; }
        public string ParentImageUri { get; protected set; }
        public string ParentMediaId { get; protected set; }
        public string ProfilePictureUri { get; protected set; }

        public int LikesCount { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        public IList<ChildComment> ChildComments { get; set; }

        protected Comment() { }

        public Comment(Guid commentId, Guid accountId, string authorPk, string author,
                        string profilePictureUri, string content, int likesCount,
                        string parentMediaId, string parentImageUri, DateTime createdAt)
        {
            SetId(commentId);
            SetAccountId(accountId);
            SetAuthorPk(authorPk);
            SetAuthor(author);
            SetProfilePictureUri(profilePictureUri);
            SetContent(content);
            SetLikesCount(likesCount);
            SetParentMediaId(parentMediaId);
            SetParentImageUri(parentImageUri);
            CreatedAt = createdAt;
        }

        private void SetId(Guid id)
        {
            if (Id == id)
            {
                return;
            }

            if (id == null)
            {
                throw new DomainException(ErrorCodes.GuidIsNull,
                    "The given Guid is null.");
            }

            if (id == Guid.Empty)
            {
                throw new DomainException(ErrorCodes.GuidIsEmpty,
                    "The given Guid is empty.");
            }

            Id = id;
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

        private void SetAuthorPk(string authorPk)
        {
            if (AuthorPk == authorPk)
            {
                return;
            }

            if (authorPk.Length < 0)
            {
                throw new DomainException(ErrorCodes.InvalidAuthorId,
                    "Comment's author's ID has to be a positive number.");
            }

            if (authorPk.Length > 15)
            {
                throw new DomainException(ErrorCodes.InvalidAuthorId,
                    "Comment's author's ID is longer than Instagram's max Pk length.");
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
                    "Comment's author's name is null!");
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new DomainException(ErrorCodes.AuthorIsEmpty,
                    "Comment's author's name is empty!");
            }

            if (author.Length > 128)
            {
                throw new DomainException(ErrorCodes.AuthorTooLong,
                    "Comment's author's name is too long!");
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

        private void SetParentMediaId(string parentMediaId)
        {
            if (ParentMediaId == parentMediaId)
            {
                return;
            }

            if (parentMediaId == null)
            {
                throw new DomainException(ErrorCodes.MediaIdIsNull,
                    "Comment's parent media's id is null!");
            }

            if (string.IsNullOrWhiteSpace(parentMediaId))
            {
                throw new DomainException(ErrorCodes.MediaIdIsEmpty,
                    "Comment's parent media's id is empty!");
            }

            if (parentMediaId.Length > 128)
            {
                throw new DomainException(ErrorCodes.MediaIdTooLong,
                    "Comment's parent media's id is too long!");
            }

            ParentMediaId = parentMediaId;
        }

        private void SetParentImageUri(string parentImageUri)
        {
            if (ParentImageUri == parentImageUri)
            {
                return;
            }

            if (parentImageUri == null)
            {
                throw new DomainException(ErrorCodes.ImageUriIsNull,
                    "Comment's parent image's uri is null!");
            }

            if (string.IsNullOrWhiteSpace(parentImageUri))
            {
                throw new DomainException(ErrorCodes.ImageUriIsEmpty,
                    "Comment's parent image's uri is empty!");
            }

            if (parentImageUri.Length > 512)
            {
                throw new DomainException(ErrorCodes.ImageUriTooLong,
                    "Comment's parent image's uri is too long!");
            }

            ParentImageUri = parentImageUri;
        }

        private void SetLikesCount(int count)
        {
            if (count < 0)
            {
                throw new DomainException(ErrorCodes.NegativeLikes,
                    "Comment's likes can't be negative");
            }

            LikesCount = count;
        }
    }
}
