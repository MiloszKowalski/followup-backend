namespace FollowUP.Infrastructure.Settings
{
    public class PromotionSettings
    {
        public bool UseProxy { get; set; }
        public bool UseLocalProxy { get; set; }
        public int StartingHour { get; set; }
        public int EndingHour { get; set; }
        public int MinActionIntervalLimit { get; set; }
        public int MaxActionIntervalLimit { get; set; }
        public int MinIntervalDifference { get; set; }
        public float MinDaysToUnfollow { get; set; }
        public string PromotionKey { get; set; }
    }
}
