using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IPromotionRepository : IRepository
    {
        Task<IEnumerable<FollowPromotion>> GetAllAsync();
        Task<FollowPromotion> GetAsync(Guid Id);
        Task<UnfollowPromotion> GetUnfollowPromotionAsync(Guid accountId);
        Task<IEnumerable<FollowPromotion>> GetAccountPromotionsAsync(Guid accountId);
        Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId);
        Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId);
        Task<FollowedProfile> GetRandomFollowedProfileAsync(Guid accountId);
        Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId);
        Task AddAsync(FollowPromotion promotion);
        Task AddAsync(UnfollowPromotion promotion);
        Task AddFollowedProfileAsync(FollowedProfile profile);
        Task AddPromotionCommentAsync(PromotionComment comment);
        Task UpdateAsync(FollowPromotion promotion);
        Task UpdateAsync(UnfollowPromotion promotion);
        Task RemoveAsync(Guid id);
        Task RemoveUnfollowPromotionAsync(Guid accountId);
        Task RemoveFollowedProfileAsync(Guid accountId, string profileId);
        Task ClearByAccountAsync(Guid accountId);
    }
}
