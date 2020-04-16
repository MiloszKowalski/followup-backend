using System;

namespace FollowUP.Core.Domain
{
    public class MonthlyBatchSchedule
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid BatchId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
