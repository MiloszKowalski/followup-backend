﻿using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using InstagramApiSharp;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    class CommentService : ICommentService
    {
        private readonly IInstagramAccountRepository _instagramAccountRepository;
        private readonly IInstagramApiService _instagramApiService;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(IInstagramAccountRepository instagramAccountRepository,
            ICommentRepository commentRepository, IMapper mapper, IInstagramApiService instagramApiService)
        {
            _instagramAccountRepository = instagramAccountRepository;
            _instagramApiService = instagramApiService;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets recent comments by given account ID with pagination 
        /// </summary>
        /// <param name="accountId">ID of the account from which the comments will be fetched</param>
        /// <param name="page">The number of page to fetch</param>
        /// <param name="pageSize">The number of comments fetched per page</param>
        /// <returns>List of paginated comments</returns>
        public async Task<IEnumerable<CommentDto>> GetByAccountIdAsync(Guid accountId, int page, int pageSize)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            // Check if the account exists
            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    "Can't get comments for the account that doesn't exist.");
            }

            // Get comments
            var comments = await _commentRepository.GetAccountCommentsAsync(accountId, page, pageSize);
            if (comments == null)
            {
                return new List<CommentDto>();
            }

            var commentDtos = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(comments);

            // Get comments' child comments and assign them to corresponding comments
            foreach (var c in commentDtos)
            {
                var childComments = await _commentRepository.GetChildCommentsAsync(c.Id);
                c.ChildComments = (List<ChildComment>)childComments;
            }

            return commentDtos;
        }

        /// <summary>
        /// Gets recent comments by given account ID
        /// </summary>
        /// <param name="accountId">ID of the account from which the comments will be fetched</param>
        /// <returns>List of all comments</returns>
        public async Task<IEnumerable<CommentDto>> GetAllByAccountIdAsync(Guid accountId)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            // Check if the account exists
            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    "Can't get comments for the account that doesn't exist.");
            }

            // Get comments
            var comments = await _commentRepository.GetAccountCommentsAsync(accountId);
            if(comments == null)
            {
                return new List<CommentDto>();
            }

            var commentDtos = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(comments);

            // Get comments' child comments and assign them to corresponding comments
            foreach (var c in commentDtos)
            {
                var childComments = await _commentRepository.GetChildCommentsAsync(c.Id);
                c.ChildComments = (List<ChildComment>)childComments;
            }

            return commentDtos;
        }

        /// <summary>
        /// Gets comments count for given account (used for pagination on the client side)
        /// </summary>
        /// <param name="accountId">ID of the account for counting comments</param>
        /// <returns>Comments count</returns>
        public async Task<int> GetCountAsync(Guid accountId)
        {
            var count = await _commentRepository.GetAccountCommentsCountAsync(accountId);
            return count;
        }

        /// <summary>
        /// Updates all of the account's comments
        /// </summary>
        /// <param name="accountId">ID of the account to update the comments</param>
        /// <returns>Bool indicating if the operation was successful</returns>
        public async Task UpdateAllByAccountIdAsync(Guid accountId)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            // Check if the account exists
            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    "Can't get comments for the account that doesn't exist.");
            }

            var instaApi = await _instagramApiService.GetInstaApiAsync(account);

            if(!instaApi.IsUserAuthenticated)
            {
                throw new ServiceException(ErrorCodes.AccountNotAuthenticated,
                    "Account not authenticated. Please login first.");
            }

            // Get given user's media (every user post)
            var userMediaResponse = await instaApi.UserProcessor.GetUserMediaAsync(account.Username,
                PaginationParameters.MaxPagesToLoad(999));

            if (!userMediaResponse.Succeeded)
            {
                throw new ServiceException(ErrorCodes.CantGetMedia,
                    "Getting user media failed. It may be Instagram's fault.");
            }

            var userMedia = userMediaResponse.Value.ToArray();

            // Clear existing comments from database, in case they've been deleted by Instagram
            await _commentRepository.ClearByAccountAsync(accountId);

            // For each post...
            foreach (var media in userMedia)
            {
                // Get post's comments
                var commentResponse = await instaApi.CommentProcessor
                                    .GetMediaCommentsAsync(media.Pk, PaginationParameters.MaxPagesToLoad(999));

                if (!commentResponse.Succeeded)
                {
                    throw new ServiceException(ErrorCodes.CantGetComments,
                        "Getting media comments failed. It may be Instagram's fault.");
                }

                var comments = commentResponse.Value.Comments;

                // For each post's comment
                foreach (var c in comments)
                {
                    // Generate unique id (also used for child comment)
                    var commentId = Guid.NewGuid();

                    // Get post's image Uri for thumbnail, according to media type
                    // TODO: Video thumbnails
                    var parentImageUri = "";
                    if (media.Images.Count > 0)
                    {
                        parentImageUri = media.Images[1].Uri;
                    }
                    else if (media.Carousel.Count > 0)
                    {
                        parentImageUri = media.Carousel[0].Images[1].Uri;
                    }

                    // Get comment's responses
                    var childComments = c.PreviewChildComments.Count > 0 ? c.PreviewChildComments : null;
                    if (childComments != null)
                    {
                        foreach (var ch in childComments)
                        {
                            // Construct the child comment and store it in database, with commentId as ParentId
                            var childComment = new ChildComment(commentId, ch.User.Pk.ToString(), ch.User.UserName,
                                ch.User.ProfilePicture, ch.Text, ch.CommentLikeCount, ch.CreatedAtUtc);

                            await _commentRepository.AddChildCommentAsync(childComment);
                        }
                    }

                    // Construct the comment and store it in database
                    var comment = new Comment(commentId, accountId, c.UserId.ToString(), c.User.UserName, c.User.ProfilePicture,
                                              c.Text, c.LikesCount, media.Pk, parentImageUri, c.CreatedAtUtc);

                    await _commentRepository.AddAsync(comment);
                }
            }
        }
    }
}
