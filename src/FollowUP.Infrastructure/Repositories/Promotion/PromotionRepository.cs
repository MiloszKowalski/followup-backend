﻿using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Settings;
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
        private readonly PromotionSettings _settings;

        public PromotionRepository(FollowUPContext context, PromotionSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        public async Task<IEnumerable<FollowPromotion>> GetAllAsync()
            => await _context.FollowPromotions.ToListAsync();

        public async Task<FollowPromotion> GetAsync(Guid id)
            => await _context.FollowPromotions.Include(x => x.InstagramAccount)
                            .ThenInclude(x => x.User)
                            .SingleOrDefaultAsync(x => x.Id == id);

        public async Task<UnfollowPromotion> GetUnfollowPromotionAsync(Guid accountId)
            => await _context.UnfollowPromotions.Include(x => x.InstagramAccount)
                            .SingleOrDefaultAsync(x => x.InstagramAccountId == accountId);  

        public async Task AddAsync(FollowPromotion promotion)
        {
            await _context.FollowPromotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(UnfollowPromotion promotion)
        {
            await _context.UnfollowPromotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FollowPromotion promotion)
        {
            _context.FollowPromotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UnfollowPromotion promotion)
        {
            _context.UnfollowPromotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var promotion = await GetAsync(id);
            _context.FollowPromotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUnfollowPromotionAsync(Guid id)
        {
            var promotion = await GetUnfollowPromotionAsync(id);
            _context.UnfollowPromotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task ClearByAccountAsync(Guid accountId)
        {
            var promotions = await GetAccountPromotionsAsync(accountId);

            if (promotions.Count() == 0)
                return;

            foreach (var promotion in promotions)
            {
                _context.FollowPromotions.Remove(promotion);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FollowPromotion>> GetAccountPromotionsAsync(Guid accountId)
            => await _context.FollowPromotions.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task<IEnumerable<PromotionComment>> GetAccountsPromotionCommentsAsync(Guid accountId)
            => await _context.PromotionComments.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task AddPromotionCommentAsync(PromotionComment comment)
        {
            await _context.PromotionComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<FollowedProfile> GetFollowedProfileAsync(Guid accountId, string profileId)
            => await _context.FollowedProfiles.SingleOrDefaultAsync(x => x.InstagramAccountId == accountId && x.ProfilePk == profileId);

        public async Task<FollowedProfile> GetRandomFollowedProfileAsync(Guid accountId)
            => await _context.FollowedProfiles.FirstOrDefaultAsync(x => x.InstagramAccountId == accountId && x.CreatedAt.AddDays(_settings.MinDaysToUnfollow) < DateTime.UtcNow);

        public async Task<IEnumerable<FollowedProfile>> GetFollowedProfilesAsync(Guid accountId)
            => await _context.FollowedProfiles.Where(x => x.InstagramAccountId == accountId).ToListAsync();

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
