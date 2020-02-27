namespace FollowUP.Infrastructure.Settings
{
    public class PromotionSettings
    {
        public bool HeadlessBrowser { get; set; }
        public int StartingHour { get; set; }
        public int EndingHour { get; set; }
        public int MinActionInterval { get; set; }
        public int MaxActionInterval { get; set; }
        public int MinIntervalDifference { get; set; }
        public int MinTooManyActionsInterval { get; set; }
        public int MaxTooManyActionsInterval { get; set; }
        public float BanDurationInDays { get; set; }
        public float MinDaysToUnfollow { get; set; }
        public int FollowChance { get; set; }
        public int UnfollowChance { get; set; }
        public bool UnfollowAfterPositiveActionsLimits { get; set; }
        public int ExploreFeedChance { get; set; }
        public int ActivityFeedChance { get; set; }
        public string PromotionKey { get; set; }
        public string ActivityKey { get; set; }
        public string SearchKey { get; set; }
    }
}
