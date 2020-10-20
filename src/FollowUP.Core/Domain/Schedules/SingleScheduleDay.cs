using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Single schedule day that may be added to a <see cref="ScheduleGroup"/>
    /// or used as <see cref="ExplicitDaySchedule"/>
    /// </summary>
    public class SingleScheduleDay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        public IList<DailyPromotionPercentage> DailyPromotionPercentages { get; set; }
        public IList<DayGroupConnection> DayGroupConnections { get; set; }
        public IList<ExplicitDaySchedule> ExplicitDaySchedules { get; set; }
    }
}
