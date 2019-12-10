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
    public class StatisticsRepository : IStatisticsRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public StatisticsRepository(FollowUPContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountStatistics>> GetAllAsync()
            => await _context.AccountStatistics.ToListAsync();

        public async Task<AccountStatistics> GetAsync(Guid id)
            => await _context.AccountStatistics.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<AccountStatistics> GetTodaysAccountStatistics(Guid accountId)
            => await _context.AccountStatistics.SingleOrDefaultAsync(x => x.AccountId == accountId && x.CreatedAt == DateTime.Today);

        public async Task<IEnumerable<AccountStatistics>> GetAllAccountStatistics(Guid accountId)
            => await _context.AccountStatistics.GroupBy(x => x.CreatedAt)
                .Select(x => new AccountStatistics(accountId, x.Key, x.Sum(s => s.ActionsCount),
                    x.Sum(s => s.LikesCount), x.Sum(s => s.FollowsCount), x.Sum(s => s.UnfollowsCount)))
                    .ToListAsync();

        public async Task AddAsync(AccountStatistics statistics)
        {
            await _context.AccountStatistics.AddAsync(statistics);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountStatistics statistics)
        {
            _context.AccountStatistics.Update(statistics);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var statistics = await GetAsync(id);
            _context.AccountStatistics.Remove(statistics);
            await _context.SaveChangesAsync();
        }
    }
}
