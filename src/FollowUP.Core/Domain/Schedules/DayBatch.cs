using System;

namespace FollowUP.Core.Domain
{
    public class DayBatch
    {
        public Guid Id { get; set; }
        public Guid ScheduleDayId { get; set; }
        public Guid BatchId { get; set; }
        public int Order { get; set; }
    }
}
