using FollowUP.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Core.Repositories
{
    public interface IScheduleRepository : IRepository
    {
        #region Monthly Batch Schedules

        Task<IEnumerable<MonthlyBatchSchedule>> GetMonthlyBatchSchedulesByAccountIdAsync(Guid accountId);
        Task<MonthlyBatchSchedule> GetMonthlyBatchScheduleByIdAsync(Guid Id);
        Task AddMonthlyBatchScheduleAsync(MonthlyBatchSchedule schedule);
        Task UpdateMonthlyBatchScheduleAsync(MonthlyBatchSchedule schedule);
        Task RemoveMonthlyBatchScheduleAsync(Guid scheduleId);

        #endregion

        #region Monthly Day Schedules

        Task<IEnumerable<MonthlyDaySchedule>> GetMonthlyDaySchedulesByAccountIdAsync(Guid accountId);
        Task<MonthlyDaySchedule> GetMonthlyDayScheduleByIdAsync(Guid Id);
        Task AddMonthlyDayScheduleAsync(MonthlyDaySchedule schedule);
        Task UpdateMonthlyDayScheduleAsync(MonthlyDaySchedule schedule);
        Task RemoveMonthlyDayScheduleAsync(Guid scheduleId);

        #endregion

        #region Schedule Batches

        Task<ScheduleBatch> GetScheduleBatchByIdAsync(Guid batchId);
        Task<IEnumerable<ScheduleBatch>> GetScheduleBatchesByAccountIdAsync(Guid accountId);
        Task AddScheduleBatchAsync(ScheduleBatch batch);
        Task UpdateScheduleBatchAsync(ScheduleBatch batch);
        Task RemoveScheduleBatchAsync(Guid batchId);

        #endregion

        #region Schedule Days

        Task<ScheduleDay> GetScheduleDayByIdAsync(Guid dayId);
        Task<IEnumerable<ScheduleDay>> GetScheduleDaysByAccountIdAsync(Guid accountId);
        Task AddScheduleDayAsync(ScheduleDay day);
        Task UpdateScheduleDayAsync(ScheduleDay day);
        Task RemoveScheduleDayAsync(Guid dayId);

        #endregion

        #region Daily Promotions

        Task<IEnumerable<DailyPromotionSchedule>> GetDailyPromotionSchedulesByDayAsync(Guid scheduleDayId);
        Task<DailyPromotionSchedule> GetDailyPromotionScheduleByIdAsync(Guid Id);
        Task AddDailyPromotionScheduleAsync(DailyPromotionSchedule dailySchedule);
        Task UpdateDailyPromotionScheduleAsync(DailyPromotionSchedule dailySchedule);
        Task RemoveDailyPromotionScheduleAsync(Guid dailyScheduleId);

        #endregion

        #region DayBatches

        Task<IEnumerable<DayBatch>> GetDayBatchesByBatchIdAsync(Guid batchId);
        Task<DayBatch> GetDayBatchByIdAsync(Guid Id);
        Task AddDayBatchAsync(DayBatch dayBatch);
        Task UpdateDayBatchAsync(DayBatch dayBatch);
        Task RemoveDayBatchAsync(Guid dayBatchId);

        #endregion
    }
}