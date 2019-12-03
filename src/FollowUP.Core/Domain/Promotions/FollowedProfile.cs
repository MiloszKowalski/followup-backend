using System;

namespace FollowUP.Core.Domain
{
    public class FollowedProfile
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public string ProfileId { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }

        protected FollowedProfile() { }

        public FollowedProfile(Guid id, Guid accountId, string profileId)
        {
            // TODO: Sanitize the fields
            Id = id;
            AccountId = accountId;
            ProfileId = profileId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
