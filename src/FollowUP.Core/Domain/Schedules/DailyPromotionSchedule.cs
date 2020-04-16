using System;

namespace FollowUP.Core.Domain
{
    public class DailyPromotionSchedule
    {
        public Guid Id { get; set; }
        public Guid ScheduleDayId { get; set; }
        public Guid PromotionId { get; set; }
        public int Percentage { get; set; }
    }
}
