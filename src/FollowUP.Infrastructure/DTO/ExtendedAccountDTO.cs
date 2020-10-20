using FollowUP.Core.Domain;
using System;

namespace FollowUP.Infrastructure.DTO
{
    public class ExtendedAccountDto : InstagramAccountDto
    {
        public string ProfileName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public string PhoneNumber { get; protected set; }
        public string AndroidDevice { get; protected set; }
        public DateTime CommentsModuleExpiry { get; protected set; }
        public DateTime PromotionsModuleExpiry { get; protected set; }
        public DateTime BannedUntil { get; protected set; }
        public AccountSettingsDto accountSettings { get; set; }
    }
}
