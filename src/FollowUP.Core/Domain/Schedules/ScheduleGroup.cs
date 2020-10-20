using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Schedule group consisting of <see cref="SingleScheduleDay"/>s
    /// that can be displayed on the schedule calendar
    /// </summary>
    public class ScheduleGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public GroupColour Colour { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        public IList<DayGroupConnection> DayGroupConnections { get; set; }
        public IList<MonthlyGroupSchedule> MonthlyGroupSchedules { get; set; }
    }
}
