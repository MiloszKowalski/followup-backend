using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IStatisticsRepository : IRepository
    {
        Task<IEnumerable<AccountStatistics>> GetAllAsync();
        Task<AccountStatistics> GetAsync(Guid Id);
        Task<AccountStatistics> GetTodaysAccountStatistics(Guid accountId);
        Task<IEnumerable<AccountStatistics>> GetAllAccountStatistics(Guid accountId);
        Task AddAsync(AccountStatistics statistics);
        Task UpdateAsync(AccountStatistics statistics);
        Task RemoveAsync(Guid id);
    }
}
