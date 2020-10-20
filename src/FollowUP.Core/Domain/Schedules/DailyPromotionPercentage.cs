using System;

namespace FollowUP.Core.Domain
{
    /// <summary>
    /// Promotion's percentage-based part of <see cref="SingleScheduleDay"/>
    /// </summary>
    public class DailyPromotionPercentage
    {
        public Guid Id { get; set; }
        public int Percentage { get; set; }

        public Guid PromotionId { get; set; }
        public Guid SingleScheduleDayId { get; set; }
        public SingleScheduleDay SingleScheduleDay { get; set; }
    }
}
