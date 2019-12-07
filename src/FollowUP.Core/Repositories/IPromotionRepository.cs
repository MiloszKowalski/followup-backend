using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IPromotionRepository : IRepository
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion> GetAsync(Guid Id);
        Task<CompletedMedia> GetMediaAsync(string code, Guid accountId);
        Task<IEnumerable<Promotion>> GetAccountPromotionsAsync(Guid accountId);
        Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId);
        Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId);
        Task<FollowedProfile> GetRandomFollowedProfileAsync(Guid accountId);
        Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId);
        Task AddAsync(Promotion promotion);
        Task AddFollowedProfileAsync(FollowedProfile profile);
        Task AddToBlacklistAsync(CompletedMedia media);
        Task AddPromotionCommentAsync(PromotionComment comment);
        Task UpdateAsync(Promotion promotion);
        Task RemoveAsync(Guid id);
        Task RemoveFollowedProfileAsync(Guid accountId, string profileId);
        Task ClearByAccount(Guid accountId);
    }
}
