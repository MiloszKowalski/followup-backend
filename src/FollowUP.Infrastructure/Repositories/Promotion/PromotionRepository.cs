using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public PromotionRepository(FollowUPContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
            => await _context.Promotions.ToListAsync();

        public async Task<Promotion> GetAsync(Guid id)
            => await _context.Promotions.SingleOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var promotion = await GetAsync(id);
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task ClearByAccount(Guid accountId)
        {
            var promotions = await GetAccountPromotionsAsync(accountId);

            if (promotions.Count() == 0)
                return;

            foreach (var promotion in promotions)
            {
                _context.Promotions.Remove(promotion);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<CompletedMedia> GetMediaAsync(string code, Guid accountId)
            => await _context.MediaBlacklist.SingleOrDefaultAsync(x => x.Code == code && x.AccountId == accountId);

        public async Task AddToBlacklistAsync(CompletedMedia media)
        {
            await _context.MediaBlacklist.AddAsync(media);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Promotion>> GetAccountPromotionsAsync(Guid accountId)
            => await _context.Promotions.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId)
            => await _context.PromotionComments.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task AddPromotionCommentAsync(PromotionComment comment)
        {
            await _context.PromotionComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId)
            => await _context.FollowedProfiles.SingleOrDefaultAsync(x => x.AccountId == accountId && x.ProfileId == profileId);

        public async Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId)
            => await _context.FollowedProfiles.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task AddFollowedProfileAsync(FollowedProfile profile)
        {
            await _context.FollowedProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFollowedProfileAsync(Guid accountId, string profileId)
        {
            var profile = await GetFollowedProfileAsync(accountId, profileId);
            _context.FollowedProfiles.Remove(profile);
            await _context.SaveChangesAsync();
        }
    }
}
