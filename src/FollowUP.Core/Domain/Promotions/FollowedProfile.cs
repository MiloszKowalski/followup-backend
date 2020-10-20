using System;

namespace FollowUP.Core.Domain
{
    public class FollowedProfile
    {
        public Guid Id { get; protected set; }
        public string ProfilePk { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        protected FollowedProfile() { }

        public FollowedProfile(Guid id, Guid accountId, string profileId)
        {
            Id = id;
            SetAccountId(accountId);
            SetProfilePk(profileId);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetAccountId(Guid accountId)
        {
            if (InstagramAccountId == accountId)
            {
                return;
            }

            if (accountId == null)
            {
                throw new DomainException(ErrorCodes.GuidIsNull,
                    "The given Account ID is null.");
            }

            if (accountId == Guid.Empty)
            {
                throw new DomainException(ErrorCodes.GuidIsEmpty,
                    "The given Account ID is empty.");
            }

            InstagramAccountId = accountId;
        }

        private void SetProfilePk(string profilePk)
        {
            if (ProfilePk == profilePk)
            {
                return;
            }

            if (profilePk == null)
            {
                throw new DomainException(ErrorCodes.ProfileIdIsNull,
                    "Followed profile's ID is null!");
            }

            if (string.IsNullOrWhiteSpace(profilePk))
            {
                throw new DomainException(ErrorCodes.ProfileIdIsEmpty,
                    "Followed profile's ID is empty!");
            }

            if (profilePk.Length > 128)
            {
                throw new DomainException(ErrorCodes.ProfileIdTooLong,
                    "Followed profile's ID is too long!");
            }

            ProfilePk = profilePk;
        }
    }
}
