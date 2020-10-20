using System;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Connection by which the <see cref="SingleScheduleDay"/>
    /// goes into <see cref="ScheduleGroup"/>
    /// </summary>
    public class DayGroupConnection
    {
        public Guid Id { get; set; }
        public int Order { get; set; }

        public Guid SingleScheduleDayId { get; set; }
        public SingleScheduleDay SingleScheduleDay { get; set; }
        public Guid ScheduleGroupId { get; set; }
        public ScheduleGroup ScheduleGroup { get; set; }
    }
}
