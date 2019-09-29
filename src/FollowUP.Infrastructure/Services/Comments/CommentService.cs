using AutoMapper;
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
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(IInstagramAccountRepository instagramAccountRepository,
                                ICommentRepository commentRepository,
                                IMapper mapper)
        {
            _instagramAccountRepository = instagramAccountRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetAllRecentByAccountId(Guid accountId)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist, "Can't get comments for the account that doesn't exist.");
            }

            if (account.AuthenticationStep != AuthenticationStep.Authenticated)
            {
                throw new ServiceException(ErrorCodes.AccountNotAuthenticated, "Account not authenticated. Please login first.");
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

        public async Task<bool> UpdateAllRecentByAccountId(Guid accountId)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, "Can't get comments for the account that doesn't exist.");

            if (account.AuthenticationStep != AuthenticationStep.Authenticated)
                throw new ServiceException(ErrorCodes.AccountNotAuthenticated, "Account not authenticated. Please login first.");

            // Create user credentials
            var userSession = new UserSessionData
            {
                UserName = account.Username,
                Password = account.Password
            };

            // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
            var instaApi = InstaApiBuilder.CreateBuilder()
                                        .SetUser(userSession)
                                        .UseLogger(new DebugLogger(LogLevel.Exceptions))
                                        .SetRequestDelay(RequestDelay.Empty())
                                        .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                        .Build();

            // Try to load the session from file
            try
            {
                instaApi?.SessionHandler?.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if(!instaApi.IsUserAuthenticated)
                throw new ServiceException(ErrorCodes.AccountNotAuthenticated, "Account not authenticated. Please login first.");

            // TODO: PaginationParemeters min and max id
            // Get given user's media (every user post)
            var userMediaResponse = await instaApi.UserProcessor.GetUserMediaAsync(account.Username, PaginationParameters.MaxPagesToLoad(999));
            if (!userMediaResponse.Succeeded)
            {
                throw new ServiceException(ErrorCodes.CantGetMedia, "Getting user media failed. It may be Instagram's fault.");
            }
            var userMedia = userMediaResponse.Value.ToArray();

            // Clear existing comments from database, in case they've been deleted by Instagram
            await _commentRepository.ClearByAccount(accountId);

            // For each post...
            foreach (var media in userMedia)
            {
                // Get post's comments
                var commentResponse = await instaApi.CommentProcessor.GetMediaCommentsAsync(media.Pk, PaginationParameters.MaxPagesToLoad(999));
                if (!commentResponse.Succeeded)
                {
                    throw new ServiceException(ErrorCodes.CantGetComments, "Getting media comments failed. It may be Instagram's fault.");
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
                        foreach (var child in childComments)
                        {
                            // Construct the child comment and store it in database, with commentId as ParentId
                            var childComment = new ChildComment(accountId, child.User.Pk, child.User.UserName, child.User.ProfilePicture,
                                                   child.Text, child.CommentLikeCount, commentId, child.CreatedAtUtc);
                            await _commentRepository.AddChildCommentAsync(childComment);
                        }
                    }

                    // Construct the comment and store it in database
                    var comment = new Comment(commentId, accountId, c.UserId, c.User.UserName, c.User.ProfilePicture,
                                              c.Text, c.LikesCount, media.Pk, parentImageUri, c.CreatedAtUtc);
                    await _commentRepository.AddAsync(comment);
                }
            }
            return true;
        }
    }
}
