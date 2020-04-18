using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    class ScheduleService : IScheduleService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMemoryCache _cache;
        private readonly Random _random;

        public ScheduleService(IScheduleRepository scheduleRepository, IPromotionRepository promotionRepository,
            IInstagramAccountRepository accountRepository, IMemoryCache cache)
        {
            _promotionRepository = promotionRepository;
            _scheduleRepository = scheduleRepository;
            _accountRepository = accountRepository;
            _random = new Random();
            _cache = cache;
        }

        public async Task SchedulePromotionQueueForTodayAsync(Guid accountId)
        {
            var promotionQueue = new Queue<PromotionTask>();
            var monthlyDaySchedules = await _scheduleRepository.GetMonthlyDaySchedulesByAccountIdAsync(accountId);
            var scheduleId = monthlyDaySchedules?.SingleOrDefault(x => x.Date == DateTime.Today)?.DayScheduleId;

            // If we found a single promotion day schedule for today
            if(scheduleId != null)
            {
                promotionQueue = await QueuePromotionTasks(accountId, scheduleId.Value);
                _cache.Set($"{accountId}-schedule", promotionQueue);
                return;
            }

            // Else, chceck the monthly batches containing a single promotion day schedule for today
            var monthlyBatchSchedules = await _scheduleRepository.GetMonthlyBatchSchedulesByAccountIdAsync(accountId);
            var monthlyBatchSchedule = monthlyBatchSchedules.SingleOrDefault(x => x.BeginDate < DateTime.Today && x.EndDate > DateTime.Today);

            scheduleId = monthlyBatchSchedule?.BatchId;

            if (scheduleId == null)
            {
                throw new ServiceException(ErrorCodes.NoScheduleAvailable,
                    "No schedule was found (for today) for the given account ID.");
            }

            var dayBatches = await _scheduleRepository.GetDayBatchesByBatchIdAsync(scheduleId.Value);
            var dayBatch = dayBatches.SingleOrDefault(x => x.Order == (DateTime.Today - monthlyBatchSchedule.BeginDate).Days);

            promotionQueue = await QueuePromotionTasks(accountId, dayBatch.ScheduleDayId);

            _cache.Set($"{accountId}-schedule", promotionQueue);
            return;
        }

        private async Task<IEnumerable<DailyPromotionSchedule>> GetTodaysPromotionSchedule(Guid scheduleId)
        {
            var dailyPromotionSchedules = await _scheduleRepository.GetDailyPromotionSchedulesByDayAsync(scheduleId);

            if(dailyPromotionSchedules == null)
            {
                throw new ServiceException(ErrorCodes.NoScheduleAvailable,
                    "The promotion schedule for the given day doesn't exist.");
            }

            int sum = 0;
            foreach (var dailyPromotionSchedule in dailyPromotionSchedules)
            {
                sum += dailyPromotionSchedule.Percentage;
            }

            if (sum != 100)
            {
                throw new ServiceException(ErrorCodes.TotalPercentageMismatch,
                    "The total percentage of all promotions was greater than 100.");
            }

            return dailyPromotionSchedules;
        }

        private async Task<Queue<PromotionTask>> QueuePromotionTasks(Guid accountId, Guid scheduleId)
        {
            var promotionQueue = new Queue<PromotionTask>();
            var dailyPromotionSchedules = await GetTodaysPromotionSchedule(scheduleId);

            var settings = await _accountRepository.GetAccountSettingsAsync(accountId);
            int maxFollows = settings.FollowsPerDay;

            foreach (var promotionSchedule in dailyPromotionSchedules)
            {
                var promotion = await _promotionRepository.GetAsync(promotionSchedule.PromotionId);

                var promotionPercentage = maxFollows * promotionSchedule.Percentage / 100;
                for (int i = 0; i < promotionPercentage; i++)
                {
                    // TODO: Get the intervals from the account's settings
                    var promotionTask = new PromotionTask(promotion, _random.Next(300, 4000));
                    promotionQueue.Enqueue(promotionTask);
                }
            }

            return promotionQueue;
        }
    }
}
