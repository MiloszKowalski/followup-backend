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
        Task<IEnumerable<PromotionCommentDto>> GetAllPromotionCommentsByAccountIdAsync(Guid accountId);
        Task<IEnumerable<PromotionDto>> GetAllPromotionsByAccountIdAsync(Guid accountId);
        Task CreatePromotionAsync(Guid accountId, PromotionType promotionType, string Label);
        Task DeletePromotionAsync(Guid promotionId);
        Task CreatePromotionCommentAsync(Guid accountId, string content);
        Task SetPromotionCooldownAsync(InstagramAccount account,
            InstagramAccountRepository accountRepository, int minActionInterval = 0, int maxActionInterval = 0);
        Task<IPromotion> GetCurrentPromotionAsync(InstagramAccount account);
    }
}
