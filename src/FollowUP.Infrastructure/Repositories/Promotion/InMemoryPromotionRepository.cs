using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryPromotionRepository : IPromotionRepository
    {
        private readonly PromotionSettings _settings;
        private static readonly List<FollowPromotion> _followPromotions = new List<FollowPromotion>();
        private static readonly List<UnfollowPromotion> _unfollowPromotions = new List<UnfollowPromotion>();
        private static readonly List<PromotionComment> _promotionComments = new List<PromotionComment>();
        private static readonly List<FollowedProfile> _followedProfiles = new List<FollowedProfile>();

        public InMemoryPromotionRepository(PromotionSettings settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<FollowPromotion>> GetAllAsync()
            => await Task.FromResult(_followPromotions);

        public async Task<FollowPromotion> GetAsync(Guid id)
            => await Task.FromResult(_followPromotions.SingleOrDefault(x => x.Id == id));

        public async Task<UnfollowPromotion> GetUnfollowPromotionAsync(Guid accountId)
            => await Task.FromResult(_unfollowPromotions.SingleOrDefault(x => x.Id == accountId));

        public async Task<IEnumerable<FollowPromotion>> GetAccountPromotionsAsync(Guid accountId)
            => await Task.FromResult(_followPromotions.Where(x => x.InstagramAccountId == accountId));

        public async Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId)
            => await Task.FromResult(_promotionComments.Where(x => x.InstagramAccountId == accountId));

        public async Task AddAsync(FollowPromotion promotion)
        {
            _followPromotions.Add(promotion);
            await Task.CompletedTask;
        }

        public async Task AddAsync(UnfollowPromotion promotion)
        {
            _unfollowPromotions.Add(promotion);
            await Task.CompletedTask;
        }

        public async Task AddPromotionCommentAsync(PromotionComment comment)
        {
            _promotionComments.Add(comment);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(FollowPromotion promotion)
        {
            await RemoveAsync(promotion.Id);
            await AddAsync(promotion);
        }

        public async Task UpdateAsync(UnfollowPromotion promotion)
        {
            await RemoveUnfollowPromotionAsync(promotion.Id);
            await AddAsync(promotion);
        }

        public async Task RemoveAsync(Guid id)
        {
            var promotion = await GetAsync(id);
            _followPromotions.Remove(promotion);
            await Task.CompletedTask;
        }

        public async Task RemoveUnfollowPromotionAsync(Guid id)
        {
            var promotion = await GetUnfollowPromotionAsync(id);
            _unfollowPromotions.Remove(promotion);
            await Task.CompletedTask;
        }

        public async Task ClearByAccountAsync(Guid accountId)
        {
            _followPromotions.Clear();
            await Task.CompletedTask;
        }

        public async Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId)
            => await Task.FromResult(_followedProfiles.SingleOrDefault(x => x.InstagramAccountId == accountId && x.ProfilePk == profileId));

        public async Task<FollowedProfile> GetRandomFollowedProfileAsync(Guid accountId)
            => await Task.FromResult(_followedProfiles.FirstOrDefault(x => x.InstagramAccountId == accountId && x.CreatedAt.AddDays(_settings.MinDaysToUnfollow) < DateTime.UtcNow));

        public async Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId)
            => await Task.FromResult(_followedProfiles.Where(x => x.InstagramAccountId == accountId));

        public async Task AddFollowedProfileAsync(FollowedProfile profile)
        {
            _followedProfiles.Add(profile);
            await Task.CompletedTask;
        }

        public async Task RemoveFollowedProfileAsync(Guid accountId, string profileId)
        {
            var profile = await GetFollowedProfileAsync(accountId, profileId);
            _followedProfiles.Remove(profile);
            await Task.CompletedTask;
        }
    }
}
