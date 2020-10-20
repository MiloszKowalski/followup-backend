using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IStatisticsService : IService
    {
        Task<AccountStatistics> GetTodayAccountStatisticsAsync(Guid accountId);
        Task<IEnumerable<AccountStatistics>> GetAllAccountStatisticsAsync(Guid accountId);
        Task<AccountStatistics> CreateEmptyAsync(Guid accountId);
        Task AddLikeAsync(Guid accountId);
        Task AddFollowAsync(Guid accountId);
        Task AddUnfollowAsync(Guid accountId);
    }
}
