using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryStatisticsRepository : IStatisticsRepository
    {
        private static readonly List<AccountStatistics> _accountStatistics = new List<AccountStatistics>();

        public async Task<IEnumerable<AccountStatistics>> GetAllAsync()
            => await Task.FromResult(_accountStatistics);

        public async Task<AccountStatistics> GetAsync(Guid id)
            => await Task.FromResult(_accountStatistics.SingleOrDefault(x => x.Id == id));

        public async Task<AccountStatistics> GetTodaysAccountStatistics(Guid accountId)
            => await Task.FromResult(_accountStatistics.SingleOrDefault(x => x.AccountId == accountId && x.CreatedAt == DateTime.Today));
        public async Task<IEnumerable<AccountStatistics>> GetAllAccountStatistics(Guid accountId)
            => await Task.FromResult(_accountStatistics.GroupBy(x => x.CreatedAt)
                .Select(x => new AccountStatistics(accountId, x.Key, x.Sum(s => s.ActionsCount),
                    x.Sum(s => s.LikesCount), x.Sum(s => s.FollowsCount), x.Sum(s => s.UnfollowsCount))));

        public async Task AddAsync(AccountStatistics statistics)
        {
            _accountStatistics.Add(statistics);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(AccountStatistics statistics)
        {
            await RemoveAsync(statistics.Id);
            await AddAsync(statistics);
        }

        public async Task RemoveAsync(Guid id)
        {
            var statistics = await GetAsync(id);
            _accountStatistics.Remove(statistics);
            await Task.CompletedTask;
        }
    }
}
