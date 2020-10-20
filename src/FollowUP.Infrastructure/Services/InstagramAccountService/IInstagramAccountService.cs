using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramAccountService : IService
    {
        Task BuyCommentsAsync(Guid accountId, double daysToAdd);
        Task BuyPromotionsAsync(Guid accountId, double daysToAdd);
        Task<IEnumerable<InstagramAccountDto>> GetAsync(int page, int pageSize);
        Task<IEnumerable<InstagramAccountDto>> GetAllByUserIdAsync(Guid userId);
        Task<InstagramAccountDto> GetExtendedInfoByIdAsync(Guid accountId);
        Task<IEnumerable<InstagramAccountDto>> GetAllByUserIdExtendedAsync(Guid userId);
        Task<int> GetCountAsync();
    }
}
