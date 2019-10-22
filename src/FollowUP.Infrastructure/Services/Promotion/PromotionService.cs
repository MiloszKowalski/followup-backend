using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public PromotionService(IPromotionRepository promotionRepository, IInstagramAccountRepository accountRepository,
            IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsByAccountId(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            var promotions = await _promotionRepository.GetAccountPromotionsAsync(accountId);

            if (promotions == null)
                throw new ServiceException(ErrorCodes.NoPromotions, $"Promotions list for the account '{accountId}' is empty.");

            var promotionDtos = _mapper.Map<IEnumerable<PromotionDto>>(promotions);

            return promotionDtos;
        }

        public async Task<IEnumerable<PromotionCommentDto>> GetAllPromotionCommentsByAccountId(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(accountId);

            if (comments == null)
                throw new ServiceException(ErrorCodes.NoComments, $"Comment list for the account '{accountId}' is empty.");

            var commentDtos = _mapper.Map<IEnumerable<PromotionCommentDto>>(comments);

            return commentDtos;
        }

        public async Task CreatePromotion(Guid accountId, PromotionType promotionType, string label)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (label.Empty())
                throw new ServiceException(ErrorCodes.LabelIsEmpty, "Label content cannot be empty.");

            if (label.Length > 100)
                throw new ServiceException(ErrorCodes.LabelTooLong, "Promotion label (hashtag or username) must be at most 100 characters long.");

            if (promotionType != PromotionType.Hashtag && promotionType != PromotionType.InstagramProfile)
                throw new ServiceException(ErrorCodes.InvalidPromotionType, "Promotion type given by user isn't supported.");

            var promotion = new Promotion(Guid.NewGuid(), accountId, promotionType, label, DateTime.UtcNow);

            await _promotionRepository.AddAsync(promotion);
        }

        public async Task CreatePromotionComment(Guid accountId, string content)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (content.Empty())
                throw new ServiceException(ErrorCodes.CommentIsEmpty, "Comment content cannot be empty.");

            if (content.Length > 100)
                throw new ServiceException(ErrorCodes.CommentTooLong, "Comment must be at most 100 characters long.");

            var comment = new PromotionComment(Guid.NewGuid(), account.Id, content, DateTime.UtcNow);

            await _promotionRepository.AddPromotionCommentAsync(comment);
        }
    }
}
