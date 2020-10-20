using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Repositories;
using FollowUP.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IPromotionRepository _promotionRepository;
        private readonly PromotionSettings _settings;

        public PromotionService(IPromotionRepository promotionRepository,
            IInstagramAccountRepository accountRepository, PromotionSettings settings,
            IMemoryCache cache, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _cache = cache;
            _mapper = mapper;
            _promotionRepository = promotionRepository;
            _settings = settings;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsByAccountIdAsync(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            var promotions = await _promotionRepository.GetAccountPromotionsAsync(accountId);

            if (promotions == null)
            {
                throw new ServiceException(ErrorCodes.NoPromotions,
                    $"Promotions list for the account '{accountId}' is empty.");
            }

            return _mapper.MapFollowPromotions(promotions);
        }

        public async Task<IEnumerable<PromotionCommentDto>> GetAllPromotionCommentsByAccountIdAsync(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(accountId);

            if (comments == null)
            {
                throw new ServiceException(ErrorCodes.NoComments,
                    $"Comment list for the account '{accountId}' is empty.");
            }

            var commentDtos = _mapper.Map<IEnumerable<PromotionCommentDto>>(comments);

            return commentDtos;
        }

        public async Task CreatePromotionAsync(Guid accountId, PromotionType promotionType, string label)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            var promotions = await _promotionRepository.GetAccountPromotionsAsync(accountId);

            foreach (var promo in promotions)
            {
                if (promo.Label == label && promo.GetPromotionType() == promotionType)
                {
                    throw new ServiceException(ErrorCodes.DuplicatePromotions,
                        $"Promotion: {label} already exists on this account.");
                }
            }

            if (label.Empty())
            {
                throw new ServiceException(ErrorCodes.LabelIsEmpty,
                    "Label content cannot be empty.");
            }

            if (label.Length > 100)
            {
                throw new ServiceException(ErrorCodes.LabelTooLong,
                    "Promotion label (hashtag or username) must be at most 100 characters long.");
            }

            if ((int)promotionType > 2)
            {
                throw new ServiceException(ErrorCodes.InvalidPromotionType,
                    "Promotion type given by user isn't supported.");
            }

            FollowPromotion promotion = null;

            switch(promotionType)
            {
                case PromotionType.Hashtag:
                    promotion = new HashtagPromotion(Guid.NewGuid(), accountId, label, DateTime.UtcNow);
                    break;
                case PromotionType.InstagramProfile:
                    promotion = new ProfilePromotion(Guid.NewGuid(), accountId, label, DateTime.UtcNow);
                    break;
                case PromotionType.Location:
                    promotion = new LocationPromotion(Guid.NewGuid(), accountId, label, DateTime.UtcNow);
                    break;
            }    

            await _promotionRepository.AddAsync(promotion);
        }

        public async Task DeletePromotionAsync(Guid promotionId)
        {
            var promotion = await _promotionRepository.GetAsync(promotionId);

            if (promotion == null)
            {
                throw new ServiceException(ErrorCodes.PromotionNotFound,
                    $"Cannot find promotion with ID: {promotionId}");
            }

            await _promotionRepository.RemoveAsync(promotionId);
        }

        public async Task CreatePromotionCommentAsync(Guid accountId, string content)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            if (content.Empty())
            {
                throw new ServiceException(ErrorCodes.CommentIsEmpty,
                    "Comment content cannot be empty.");
            }

            if (content.Length > 100)
            {
                throw new ServiceException(ErrorCodes.CommentTooLong,
                    "Comment must be at most 100 characters long.");
            }

            var comment = new PromotionComment(Guid.NewGuid(), account.Id, content, DateTime.UtcNow);

            await _promotionRepository.AddPromotionCommentAsync(comment);
        }
        public async Task SetPromotionCooldownAsync(InstagramAccount account, InstagramAccountRepository accountRepository,
            int minActionInterval = 0, int maxActionInterval = 0)
        { 
            var rand = new Random();
            int milliseconds = 0;
            int previousMilliseconds = 0;

            if(minActionInterval <= 0 || maxActionInterval <= 0 || minActionInterval > maxActionInterval)
            {
                milliseconds = rand.Next(_settings.MinActionIntervalLimit, _settings.MaxActionIntervalLimit);
                previousMilliseconds = account.PreviousCooldownMilliseconds;

                // If the difference between this interval and the previous
                // one is less than given number of milliseconds, randomize interval again
                while (Math.Abs(previousMilliseconds - milliseconds) < _settings.MinIntervalDifference)
                {
                    milliseconds = rand.Next(_settings.MinActionIntervalLimit, _settings.MaxActionIntervalLimit);
                }
            }
            else
            {
                milliseconds = rand.Next(minActionInterval, maxActionInterval);
            }

            account.SetActionCooldown(milliseconds);

            await accountRepository.UpdateAsync(account);
            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Waiting {milliseconds} milliseconds");
        }

        public async Task<IPromotion> GetCurrentPromotionAsync(InstagramAccount account)
        {
            // Get all account's promotions
            string promotionKey = $"{account.Id}{_settings.PromotionKey}";
            var promotions = await _promotionRepository.GetAccountPromotionsAsync(account.Id);

            if (promotions == null || !promotions.Any())
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Couldn't find any promotions, skipping");
                await Task.Delay(TimeSpan.FromSeconds(5));
                return null;
            }

            var promotionList = promotions.OrderBy(x => x.Label).ToList();

            // Queue the promotions to make only one per iteration
            var currentPromotion = promotionList.First();

            var previousPromotion = (IPromotion)_cache.Get(promotionKey);

            if (previousPromotion != null)
            {
                int previousPromotionIndex = -1;
                foreach (var promo in promotionList)
                {
                    if (promo.Id == previousPromotion.Id)
                    {
                        previousPromotionIndex = promotionList.IndexOf(promo);
                        break;
                    }
                }
                currentPromotion = promotionList[previousPromotionIndex >= promotionList.Count - 1 ? 0 : (previousPromotionIndex + 1)];
            }
            _cache.Set(promotionKey, currentPromotion);

            return currentPromotion;
        }
    }
}
