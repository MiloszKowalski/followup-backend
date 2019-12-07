namespace FollowUP.Infrastructure.Settings
{
    public class PromotionSettings
    {
        public bool UseEmbeddedBrowser { get; set; }
        public bool HeadlessBrowser { get; set; }
        public int MinActionInterval { get; set; }
        public int MaxActionInterval { get; set; }
        public int MinIntervalDifference { get; set; }
        public float BanDurationInDays { get; set; }
        public float MinDaysToUnfollow { get; set; }
        public string PromotionKey { get; set; }
        public string ActivityKey { get; set; }
        public string SearchKey { get; set; }
    }
}
