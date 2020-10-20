using System;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Monthly schedule that gives <see cref="ScheduleGroup"/>
    /// a beginning and ending date
    /// </summary>
    public class MonthlyGroupSchedule
    {
        public Guid Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        public Guid ScheduleGroupId { get; set; }
        public ScheduleGroup ScheduleGroup { get; set; }
    }
}
