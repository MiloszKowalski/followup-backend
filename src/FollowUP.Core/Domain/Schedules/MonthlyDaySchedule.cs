using System;

namespace FollowUP.Core.Domain
{
    public class MonthlyDaySchedule
    {
        public Guid Id { get; set; }
        public Guid DayScheduleId { get; set; }
        public Guid AccountId { get; set; }
        public DateTime Date { get; set; }
    }
}
