namespace FollowUP.Infrastructure.DTO
{
    public class AccountSettingsDto
    {
        public int ActionsPerDay { get; set; }
        public int FollowsPerDay { get; set; }
        public int LikesPerDay { get; set; }
        public int UnfollowsPerDay { get; set; }
        public int MinIntervalMilliseconds { get; set; }
        public int MaxIntervalMilliseconds { get; set; }
    }
}
