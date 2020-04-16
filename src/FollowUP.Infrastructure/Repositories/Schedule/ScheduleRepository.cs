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
    public class ScheduleRepository : IScheduleRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public ScheduleRepository(FollowUPContext context)
        {
            _context = context;
        }

        #region Monthly Batch Schedules

        public async Task<IEnumerable<MonthlyBatchSchedule>> GetMonthlyBatchSchedulesByAccountIdAsync(Guid accountId)
            => await _context.MonthlyBatchSchedules.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task<MonthlyBatchSchedule> GetMonthlyBatchScheduleByIdAsync(Guid Id)
            => await _context.MonthlyBatchSchedules.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddMonthlyBatchScheduleAsync(MonthlyBatchSchedule schedule)
        {
            await _context.MonthlyBatchSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMonthlyBatchScheduleAsync(MonthlyBatchSchedule schedule)
        {
            _context.MonthlyBatchSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMonthlyBatchScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetMonthlyBatchScheduleByIdAsync(scheduleId);
            _context.MonthlyBatchSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Monthly Day Schedules

        public async Task<IEnumerable<MonthlyDaySchedule>> GetMonthlyDaySchedulesByAccountIdAsync(Guid accountId)
            => await _context.MonthlyDaySchedules.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task<MonthlyDaySchedule> GetMonthlyDayScheduleByIdAsync(Guid Id)
            => await _context.MonthlyDaySchedules.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddMonthlyDayScheduleAsync(MonthlyDaySchedule schedule)
        {
            await _context.MonthlyDaySchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMonthlyDayScheduleAsync(MonthlyDaySchedule schedule)
        {
            _context.MonthlyDaySchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMonthlyDayScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetMonthlyDayScheduleByIdAsync(scheduleId);
            _context.MonthlyDaySchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Schedule Batches

        public async Task<ScheduleBatch> GetScheduleBatchByIdAsync(Guid batchId)
            => await _context.ScheduleBatches.SingleOrDefaultAsync(x => x.Id == batchId);

        public async Task<IEnumerable<ScheduleBatch>> GetScheduleBatchesByAccountIdAsync(Guid accountId)
            => await _context.ScheduleBatches.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task AddScheduleBatchAsync(ScheduleBatch batch)
        {
            await _context.ScheduleBatches.AddAsync(batch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScheduleBatchAsync(ScheduleBatch batch)
        {
            _context.ScheduleBatches.Update(batch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveScheduleBatchAsync(Guid batchId)
        {
            var batch = await GetScheduleBatchByIdAsync(batchId);
            _context.ScheduleBatches.Remove(batch);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Schedule Days

        public async Task<ScheduleDay> GetScheduleDayByIdAsync(Guid dayId)
            => await _context.ScheduleDays.SingleOrDefaultAsync(x => x.Id == dayId);

        public async Task<IEnumerable<ScheduleDay>> GetScheduleDaysByAccountIdAsync(Guid accountId)
            => await _context.ScheduleDays.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task AddScheduleDayAsync(ScheduleDay day)
        {
            await _context.ScheduleDays.AddAsync(day);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScheduleDayAsync(ScheduleDay day)
        {
            _context.Update(day);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveScheduleDayAsync(Guid dayId)
        {
            var day = await GetScheduleDayByIdAsync(dayId);
            _context.ScheduleDays.Remove(day);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Daily Promotions

        public async Task<IEnumerable<DailyPromotionSchedule>> GetDailyPromotionSchedulesByDayAsync(Guid scheduleDayId)
            => await _context.DailyPromotionSchedules.Where(x => x.ScheduleDayId == scheduleDayId).ToListAsync();

        public async Task<DailyPromotionSchedule> GetDailyPromotionScheduleByIdAsync(Guid Id)
            => await _context.DailyPromotionSchedules.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddDailyPromotionScheduleAsync(DailyPromotionSchedule dailySchedule)
        {
            await _context.DailyPromotionSchedules.AddAsync(dailySchedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDailyPromotionScheduleAsync(DailyPromotionSchedule dailySchedule)
        {
            _context.Update(dailySchedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDailyPromotionScheduleAsync(Guid dailyScheduleId)
        {
            var dailySchedule = await GetDailyPromotionScheduleByIdAsync(dailyScheduleId);
            _context.DailyPromotionSchedules.Remove(dailySchedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region DayBatches

        public async Task<IEnumerable<DayBatch>> GetDayBatchesByBatchIdAsync(Guid batchId)
            => await _context.DayBatches.Where(x => x.BatchId == batchId).ToListAsync();

        public async Task<DayBatch> GetDayBatchByIdAsync(Guid Id)
            => await _context.DayBatches.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddDayBatchAsync(DayBatch dayBatch)
        {
            await _context.DayBatches.AddAsync(dayBatch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDayBatchAsync(DayBatch dayBatch)
        {
            _context.DayBatches.Update(dayBatch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDayBatchAsync(Guid dayBatchId)
        {
            var dayBatch = await GetDayBatchByIdAsync(dayBatchId);
            _context.DayBatches.Remove(dayBatch);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
