using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Repositories;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IPromotionService : IService
    {
        Task<IEnumerable<PromotionCommentDto>> GetAllPromotionCommentsByAccountId(Guid accountId);
        Task<IEnumerable<PromotionDto>> GetAllPromotionsByAccountId(Guid accountId);
        Task CreatePromotion(Guid accountId, PromotionType promotionType, string Label);
        Task CreatePromotionComment(Guid accountId, string content);
        Task SetPromotionCooldown(InstagramAccount account, InstagramAccountRepository accountRepository, int minActionInterval = 0, int maxActionInterval = 0);
        Task ReLoginUser(InstagramAccount account);
        Task ProceedBan(InstagramAccount account);
        Task<bool> LookupActivityFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion = null);
        Task<bool> LookupExploreFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion = null);
        Task<bool> LikeMedia(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, StatisticsService statisticsService, InstaMedia media, int likesDone);
        Task<bool> FollowProfile(IInstaApi instaApi, InstagramAccount account, Promotion promotion, PromotionRepository promotionRepository,
            StatisticsService statisticsService, InstagramAccountRepository accountRepository, InstaMedia media, int unFollowsDone);
        Task<bool> UnfollowProfile(IInstaApi instaApi, InstagramAccount account, PromotionRepository promotionRepository,
            StatisticsService statisticsService, InstagramAccountRepository accountRepository, int unFollowsDone);
        Task<Promotion> GetCurrentPromotion(InstagramAccount account);
    }
}
