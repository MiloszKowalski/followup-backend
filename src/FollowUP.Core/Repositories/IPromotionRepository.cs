﻿using System;
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
        Task AddAsync(Promotion promotion);
        Task AddToBlacklistAsync(CompletedMedia media);
        Task AddPromotionCommentAsync(PromotionComment comment);
        Task UpdateAsync(Promotion promotion);
        Task RemoveAsync(Guid id);
        Task ClearByAccount(Guid accountId);
    }
}