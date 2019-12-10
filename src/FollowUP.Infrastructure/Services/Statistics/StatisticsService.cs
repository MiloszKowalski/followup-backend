using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }
        public async Task AddFollow(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatistics(accountId);
            statistics.AddFollow();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task AddLike(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatistics(accountId);
            statistics.AddLike();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task AddUnfollow(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatistics(accountId);
            statistics.AddUnfollow();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task<IEnumerable<AccountStatistics>> GetAllAccountStatistics(Guid accountId)
        {
            // TODO: DTO
            return await _statisticsRepository.GetAllAccountStatistics(accountId);
        }

        public async Task<AccountStatistics> GetTodayAccountStatistics(Guid accountId)
        {
            // TODO: DTO
            return await _statisticsRepository.GetTodaysAccountStatistics(accountId);
        }
    }
}
