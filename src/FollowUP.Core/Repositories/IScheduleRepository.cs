using FollowUP.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Core.Repositories
{
    public interface IScheduleRepository : IRepository
    {
        #region Monthly Group Schedules

        Task<IEnumerable<MonthlyGroupSchedule>> GetMonthlyGroupSchedulesByAccountIdAsync(Guid accountId);
        Task<MonthlyGroupSchedule> GetMonthlyGroupScheduleByIdAsync(Guid Id);
        Task AddMonthlyGroupScheduleAsync(MonthlyGroupSchedule schedule);
        Task UpdateMonthlyGroupScheduleAsync(MonthlyGroupSchedule schedule);
        Task RemoveMonthlyGroupScheduleAsync(Guid scheduleId);

        #endregion

        #region Explicit Day Schedules

        Task<IEnumerable<ExplicitDaySchedule>> GetExplicitDaySchedulesByAccountIdAsync(Guid accountId);
        Task<IEnumerable<ScheduleGroup>> GetExplicitScheduleForToday(Guid accountId);
        Task<ExplicitDaySchedule> GetExplicitDayScheduleByIdAsync(Guid Id);
        Task AddExplicitDayScheduleAsync(ExplicitDaySchedule schedule);
        Task UpdateExplicitDayScheduleAsync(ExplicitDaySchedule schedule);
        Task RemoveExplicitDayScheduleAsync(Guid scheduleId);

        #endregion

        #region Schedule Groups

        Task<ScheduleGroup> GetScheduleGroupByIdAsync(Guid batchId);
        Task<IEnumerable<ScheduleGroup>> GetScheduleGroupsByAccountIdAsync(Guid accountId);
        Task AddScheduleGroupAsync(ScheduleGroup group);
        Task UpdateScheduleGroupAsync(ScheduleGroup group);
        Task RemoveScheduleGroupAsync(Guid groupId);
        Task RemoveMultipleScheduleGroupsAsync(IEnumerable<Guid> guids);

        #endregion

        #region Single Schedule Days

        Task<SingleScheduleDay> GetSingleScheduleDayByIdAsync(Guid dayId);
        Task<IEnumerable<SingleScheduleDay>> GetSingleScheduleDaysByAccountIdAsync(Guid accountId);
        Task AddSingleScheduleDayAsync(SingleScheduleDay day);
        Task UpdateSingleScheduleDayAsync(SingleScheduleDay day);
        Task RemoveSingleScheduleDayAsync(Guid dayId);
        Task RemoveMultipleSingleScheduleDaysAsync(IEnumerable<Guid> dayIds);

        #endregion

        #region Daily Promotion Percentages

        Task<IEnumerable<DailyPromotionPercentage>> GetDailyPromotionPercentagesByDayAsync(Guid scheduleDayId);
        Task<DailyPromotionPercentage> GetDailyPromotionPercentageByIdAsync(Guid Id);
        Task AddDailyPromotionPercentageAsync(DailyPromotionPercentage dailyPromotionPercentage);
        Task UpdateDailyPromotionPercentageAsync(DailyPromotionPercentage dailyPromotionPercentage);
        Task RemoveDailyPromotionPercentageAsync(Guid dailyPromotionPercentageId);

        #endregion

        #region DayGroupConnection

        Task<IEnumerable<DayGroupConnection>> GetDayGroupConnectionsByGroupIdAsync(Guid groupId);
        Task<DayGroupConnection> GetDayGroupConnectionByIdAsync(Guid Id);
        Task AddDayGroupConnectionAsync(DayGroupConnection dayGroupConnection);
        Task UpdateDayGroupConnectionAsync(DayGroupConnection dayGroupConnection);
        Task RemoveDayGroupConnectionAsync(Guid dayGroupConnectionId);

        #endregion
    }
}