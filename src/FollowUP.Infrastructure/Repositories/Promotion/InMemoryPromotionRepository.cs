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
        private static readonly List<Promotion> _promotions = new List<Promotion>();
        private static readonly List<PromotionComment> _promotionComments = new List<PromotionComment>();
        private static readonly List<CompletedMedia> _media = new List<CompletedMedia>();
        private static readonly List<FollowedProfile> _followedProfiles = new List<FollowedProfile>();

        public InMemoryPromotionRepository(PromotionSettings settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
            => await Task.FromResult(_promotions);

        public async Task<Promotion> GetAsync(Guid id)
            => await Task.FromResult(_promotions.SingleOrDefault(x => x.Id == id));

        public async Task<CompletedMedia> GetMediaAsync(string code, Guid accountId)
            => await Task.FromResult(_media.SingleOrDefault(x => x.Code == code && x.AccountId == accountId));

        public async Task<IEnumerable<Promotion>> GetAccountPromotionsAsync(Guid accountId)
            => await Task.FromResult(_promotions.Where(x => x.AccountId == accountId));

        public async Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId)
            => await Task.FromResult(_promotionComments.Where(x => x.AccountId == accountId));

        public async Task AddAsync(Promotion promotion)
        {
            _promotions.Add(promotion);
            await Task.CompletedTask;
        }

        public async Task AddToBlacklistAsync(CompletedMedia media)
        {
            _media.Add(media);
            await Task.CompletedTask;
        }

        public async Task AddPromotionCommentAsync(PromotionComment comment)
        {
            _promotionComments.Add(comment);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            await RemoveAsync(promotion.Id);
            await AddAsync(promotion);
        }

        public async Task RemoveAsync(Guid id)
        {
            var promotion = await GetAsync(id);
            _promotions.Remove(promotion);
            await Task.CompletedTask;
        }

        public async Task ClearByAccount(Guid accountId)
        {
            _promotions.Clear();
            await Task.CompletedTask;
        }

        public async Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId)
            => await Task.FromResult(_followedProfiles.SingleOrDefault(x => x.AccountId == accountId && x.ProfileId == profileId));

        public async Task<FollowedProfile> GetRandomFollowedProfileAsync(Guid accountId)
            => await Task.FromResult(_followedProfiles.FirstOrDefault(x => x.AccountId == accountId && x.CreatedAt.AddDays(_settings.MinDaysToUnfollow) < DateTime.UtcNow));

        public async Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId)
            => await Task.FromResult(_followedProfiles.Where(x => x.AccountId == accountId));

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
