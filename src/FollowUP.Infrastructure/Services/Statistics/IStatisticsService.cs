using FollowUP.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IStatisticsService : IService
    {
        Task<AccountStatistics> GetTodayAccountStatistics(Guid accountId);
        Task<IEnumerable<AccountStatistics>> GetAllAccountStatistics(Guid accountId);
        Task AddLike(Guid accountId);
        Task AddFollow(Guid accountId);
        Task AddUnfollow(Guid accountId);
    }
}
