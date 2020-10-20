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

        /// <summary>
        /// Gets the defined schedules from database, creates
        /// <see cref="PromotionTask"/> queue for the current day
        /// and stores it in cache
        /// </summary>
        /// <param name="accountId">ID of the account for which the schedule is created</param>
        /// <returns></returns>
        public async Task SchedulePromotionQueueForTodayAsync(Guid accountId)
        {
            var promotionQueue = new Queue<PromotionTask>();
            var explicitDaySchedules = await _scheduleRepository.GetExplicitDaySchedulesByAccountIdAsync(accountId);
            var scheduleId = explicitDaySchedules?.SingleOrDefault(x => x.Date == DateTime.Today)?.SingleScheduleDayId;

            // If we've found a single promotion day schedule for today...
            if(scheduleId != null)
            {
                // ...get the promotion queue and exit function
                promotionQueue = await QueuePromotionTasksAsync(accountId, scheduleId.Value);
                _cache.Set($"{accountId}-schedule", promotionQueue);
                _cache.Set($"{accountId}-schedule-changed", true);
                return;
            }

            // Else, check the monthly batches containing a single promotion day schedule for today
            var monthlyGroupSchedules = await _scheduleRepository.GetMonthlyGroupSchedulesByAccountIdAsync(accountId);
            var monthlyGroupSchedule = monthlyGroupSchedules
                .SingleOrDefault(x => x.BeginDate < DateTime.Today && x.EndDate > DateTime.Today);

            scheduleId = monthlyGroupSchedule?.ScheduleGroupId;

            if (scheduleId == null)
            {
                throw new ServiceException(ErrorCodes.NoScheduleAvailable,
                    "No schedule was found (for today) for the given account ID.");
            }

            // Get the day batch connection to find the given day
            var dayBatches = await _scheduleRepository.GetDayGroupConnectionsByGroupIdAsync(scheduleId.Value);
            var promotionDaySpan = (DateTime.Today - monthlyGroupSchedule.BeginDate).Days;
            var dayBatch = dayBatches.SingleOrDefault(x => x.Order == promotionDaySpan % dayBatches.Count());

            promotionQueue = await QueuePromotionTasksAsync(accountId, dayBatch.SingleScheduleDayId);

            _cache.Set($"{accountId}-schedule", promotionQueue);
            _cache.Set($"{accountId}-schedule-changed", true);
            return;
        }

        /// <summary>
        /// Gets promotion schedule for the given day, on promotion-with-percentage basis
        /// </summary>
        /// <param name="singleScheduleDayId">ID of the <see cref="SingleScheduleDay"/> for
        /// which the schedule belongs to</param>
        /// <returns>List of the promotion-with-percentage for the given day</returns>
        private async Task<IEnumerable<DailyPromotionPercentage>>GetTodaysPromotionScheduleAsync(Guid singleScheduleDayId)
        {
            var dailyPromotionSchedules = await _scheduleRepository
                                                .GetDailyPromotionPercentagesByDayAsync(singleScheduleDayId);

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
                    "The total percentage of all promotions was not equalt to 100.");
            }

            return dailyPromotionSchedules;
        }

        /// <summary>
        /// Queues <see cref="PromotionTask"/> based on the <see cref="SingleScheduleDay"/>
        /// </summary>
        /// <param name="accountId">ID of the account for which the queue is created</param>
        /// <param name="singleScheduleDayId">ID of the <see cref="SingleScheduleDay"/>
        /// for which the queue is created</param>
        /// <returns>Queue of <see cref="PromotionTask"/> for the given day</returns>
        private async Task<Queue<PromotionTask>> QueuePromotionTasksAsync(Guid accountId, Guid singleScheduleDayId)
        {
            var promotionQueue = new Queue<PromotionTask>();
            var dailyPromotionPercentages = await GetTodaysPromotionScheduleAsync(singleScheduleDayId);

            var settings = await _accountRepository.GetAccountSettingsAsync(accountId);
            var currentQueue = (Queue<PromotionTask>)_cache.Get($"{accountId}-schedule");
            int maxFollows = settings.FollowsPerDay;

            int actionsToSkip = 0;
            if (currentQueue != null)
            {
                actionsToSkip = maxFollows - currentQueue.Count();
            }

            int actionsSkipped = 0;

            // Calculate how many actions to do with the given promotion
            foreach (var promotionSchedule in dailyPromotionPercentages)
            {
                var promotion = await _promotionRepository.GetAsync(promotionSchedule.PromotionId);

                var promotionTasksCount = maxFollows * promotionSchedule.Percentage / 100;
                for (int i = 0; i < promotionTasksCount; i++)
                {
                    if (actionsSkipped < actionsToSkip)
                    {
                        actionsSkipped++;
                        continue;
                    }

                    var promotionTask = new PromotionTask(promotion,
                        _random.Next(settings.MinIntervalMilliseconds, settings.MaxIntervalMilliseconds));

                    promotionQueue.Enqueue(promotionTask);
                }
            }

            return promotionQueue;
        }
    }
}
