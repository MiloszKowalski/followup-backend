namespace FollowUP.Infrastructure.Settings
{
    public class PromotionSettings
    {
        public bool UseEmbeddedBrowser { get; set; }
        public bool HeadlessBrowser { get; set; }
        public int MinActionInterval { get; set; }
        public int MaxActionInterval { get; set; }
        public int MinIntervalDifference { get; set; }
    }
}
