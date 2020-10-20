using System;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Explicit single day schedule composed of <see cref="SingleScheduleDay"/>
    /// and a date at which the schedule will be processed. It has a higher
    /// priority than every other monthly-based or group-based schedule.
    /// </summary>
    public class ExplicitDaySchedule
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        public Guid SingleScheduleDayId { get; set; }
        public SingleScheduleDay SingleScheduleDay { get; set; }
    }
}
