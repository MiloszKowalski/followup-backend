using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
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
    }
}
