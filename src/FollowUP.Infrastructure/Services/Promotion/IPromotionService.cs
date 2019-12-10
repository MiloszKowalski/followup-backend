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
        Task SetPromotionCooldown(InstagramAccount account, InstagramAccountRepository accountRepository);
        Task ReLoginUser(InstagramAccount account);
        Task ProceedBan(InstagramAccount account);
        Task<bool> LookupActivityFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion);
        Task<bool> LookupExploreFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion);
        Task LikeMedia(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, StatisticsService statisticsService, InstaMedia media, int likesDone);
        Task FollowProfile(IInstaApi instaApi, InstagramAccount account, Promotion promotion, PromotionRepository promotionRepository,
            StatisticsService statisticsService, InstagramAccountRepository accountRepository, InstaMedia media, int unFollowsDone);
        Task<bool> UnfollowProfile(IInstaApi instaApi, InstagramAccount account, PromotionRepository promotionRepository,
            StatisticsService statisticsService, InstagramAccountRepository accountRepository, int unFollowsDone);
        Task<IInstaApi> GetInstaApi(InstagramAccount account);
        Task<Promotion> GetCurrentPromotion(InstagramAccount account);
    }
}
