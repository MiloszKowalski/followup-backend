using System;

namespace FollowUP.Core.Domain
{
    public class ScheduleDay
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
    }
}
