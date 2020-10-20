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

        #region MonthlyGroupSchedules

        public async Task<IEnumerable<MonthlyGroupSchedule>> GetMonthlyGroupSchedulesByAccountIdAsync(Guid accountId)
            => await _context.MonthlyGroupSchedules.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task<MonthlyGroupSchedule> GetMonthlyGroupScheduleByIdAsync(Guid Id)
            => await _context.MonthlyGroupSchedules.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddMonthlyGroupScheduleAsync(MonthlyGroupSchedule schedule)
        {
            await _context.MonthlyGroupSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMonthlyGroupScheduleAsync(MonthlyGroupSchedule schedule)
        {
            _context.MonthlyGroupSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMonthlyGroupScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetMonthlyGroupScheduleByIdAsync(scheduleId);
            _context.MonthlyGroupSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region ExplicitDaySchedules

        public async Task<IEnumerable<ExplicitDaySchedule>> GetExplicitDaySchedulesByAccountIdAsync(Guid accountId)
            => await _context.ExplicitDaySchedules.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task<IEnumerable<ScheduleGroup>> GetExplicitScheduleForToday(Guid accountId)
            => await _context.ScheduleGroups.Where(x => x.InstagramAccountId == accountId)
                .Include(sg => sg.DayGroupConnections)
                .ThenInclude(dgc => dgc.SingleScheduleDay)
                .ThenInclude(ssd => ssd.DailyPromotionPercentages)
                .ToListAsync();

        public async Task<ExplicitDaySchedule> GetExplicitDayScheduleByIdAsync(Guid Id)
            => await _context.ExplicitDaySchedules.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddExplicitDayScheduleAsync(ExplicitDaySchedule schedule)
        {
            await _context.ExplicitDaySchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExplicitDayScheduleAsync(ExplicitDaySchedule schedule)
        {
            _context.ExplicitDaySchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveExplicitDayScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetExplicitDayScheduleByIdAsync(scheduleId);
            _context.ExplicitDaySchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region ScheduleGroups

        public async Task<ScheduleGroup> GetScheduleGroupByIdAsync(Guid batchId)
            => await _context.ScheduleGroups.SingleOrDefaultAsync(x => x.Id == batchId);

        public async Task<IEnumerable<ScheduleGroup>> GetScheduleGroupsByAccountIdAsync(Guid accountId)
            => await _context.ScheduleGroups.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task AddScheduleGroupAsync(ScheduleGroup batch)
        {
            await _context.ScheduleGroups.AddAsync(batch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScheduleGroupAsync(ScheduleGroup batch)
        {
            _context.ScheduleGroups.Update(batch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveScheduleGroupAsync(Guid batchId)
        {
            var batch = await GetScheduleGroupByIdAsync(batchId);
            _context.ScheduleGroups.Remove(batch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMultipleScheduleGroupsAsync(IEnumerable<Guid> ids)
        {
            foreach (Guid id in ids)
            {
                var scheduleGroup = await GetScheduleGroupByIdAsync(id);
                _context.ScheduleGroups.Remove(scheduleGroup);
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region SingleScheduleDays

        public async Task<SingleScheduleDay> GetSingleScheduleDayByIdAsync(Guid dayId)
            => await _context.SingleScheduleDays.SingleOrDefaultAsync(x => x.Id == dayId);

        public async Task<IEnumerable<SingleScheduleDay>> GetSingleScheduleDaysByAccountIdAsync(Guid accountId)
            => await _context.SingleScheduleDays.Where(x => x.InstagramAccountId == accountId).ToListAsync();

        public async Task AddSingleScheduleDayAsync(SingleScheduleDay day)
        {
            await _context.SingleScheduleDays.AddAsync(day);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSingleScheduleDayAsync(SingleScheduleDay day)
        {
            _context.Update(day);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSingleScheduleDayAsync(Guid dayId)
        {
            var day = await GetSingleScheduleDayByIdAsync(dayId);
            _context.SingleScheduleDays.Remove(day);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMultipleSingleScheduleDaysAsync(IEnumerable<Guid> dayIds)
        {
            foreach (Guid id in dayIds)
            {
                var singleScheduleDay = await GetSingleScheduleDayByIdAsync(id);
                _context.SingleScheduleDays.Remove(singleScheduleDay);
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region DailyPromotionPercentages

        public async Task<IEnumerable<DailyPromotionPercentage>> GetDailyPromotionPercentagesByDayAsync(Guid scheduleDayId)
            => await _context.DailyPromotionPercentages.Where(x => x.SingleScheduleDayId == scheduleDayId).ToListAsync();

        public async Task<DailyPromotionPercentage> GetDailyPromotionPercentageByIdAsync(Guid Id)
            => await _context.DailyPromotionPercentages.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddDailyPromotionPercentageAsync(DailyPromotionPercentage dailySchedule)
        {
            await _context.DailyPromotionPercentages.AddAsync(dailySchedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDailyPromotionPercentageAsync(DailyPromotionPercentage dailySchedule)
        {
            _context.Update(dailySchedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDailyPromotionPercentageAsync(Guid dailyScheduleId)
        {
            var dailySchedule = await GetDailyPromotionPercentageByIdAsync(dailyScheduleId);
            _context.DailyPromotionPercentages.Remove(dailySchedule);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region DayGroupConnections

        public async Task<IEnumerable<DayGroupConnection>> GetDayGroupConnectionsByGroupIdAsync(Guid batchId)
            => await _context.DayGroupConnections.Where(x => x.ScheduleGroupId == batchId).ToListAsync();

        public async Task<DayGroupConnection> GetDayGroupConnectionByIdAsync(Guid Id)
            => await _context.DayGroupConnections.SingleOrDefaultAsync(x => x.Id == Id);

        public async Task AddDayGroupConnectionAsync(DayGroupConnection dayBatch)
        {
            await _context.DayGroupConnections.AddAsync(dayBatch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDayGroupConnectionAsync(DayGroupConnection dayBatch)
        {
            _context.DayGroupConnections.Update(dayBatch);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDayGroupConnectionAsync(Guid dayBatchId)
        {
            var dayBatch = await GetDayGroupConnectionByIdAsync(dayBatchId);
            _context.DayGroupConnections.Remove(dayBatch);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
