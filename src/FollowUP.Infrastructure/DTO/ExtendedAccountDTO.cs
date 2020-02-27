using System;

namespace FollowUP.Infrastructure.DTO
{
    public class ExtendedAccountDto : AccountDto
    {
        public string ProfileName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public int ActionsPerDay { get; set; }
        public int LikesPerDay { get; set; }
        public int FollowsPerDay { get; set; }
        public int UnfollowsPerDay { get; set; }
        public string PhoneNumber { get; protected set; }
        public string AndroidDevice { get; protected set; }
        public DateTime CommentsModuleExpiry { get; protected set; }
        public DateTime PromotionsModuleExpiry { get; protected set; }
        public DateTime BannedUntil { get; protected set; }
    }
}
