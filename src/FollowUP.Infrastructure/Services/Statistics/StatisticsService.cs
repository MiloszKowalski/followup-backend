using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
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

        public async Task<AccountStatistics> CreateEmptyAsync(Guid accountId)
        {
            var statistics = new AccountStatistics(Guid.NewGuid(), accountId);
            await _statisticsRepository.AddAsync(statistics);
            return statistics;
        }

        public async Task AddFollowAsync(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatisticsAsync(accountId);
            statistics.AddFollow();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task AddLikeAsync(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatisticsAsync(accountId);
            statistics.AddLike();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task AddUnfollowAsync(Guid accountId)
        {
            var statistics = await _statisticsRepository.GetTodaysAccountStatisticsAsync(accountId);
            statistics.AddUnfollow();
            statistics.AddAction();
            await _statisticsRepository.UpdateAsync(statistics);
        }

        public async Task<IEnumerable<AccountStatistics>> GetAllAccountStatisticsAsync(Guid accountId)
        {
            return await _statisticsRepository.GetAllAccountStatisticsAsync(accountId);
        }

        public async Task<AccountStatistics> GetTodayAccountStatisticsAsync(Guid accountId)
        {
            return await _statisticsRepository.GetTodaysAccountStatisticsAsync(accountId);
        }
    }
}
