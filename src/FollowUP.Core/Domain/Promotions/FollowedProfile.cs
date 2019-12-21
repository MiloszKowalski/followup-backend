using System;

namespace FollowUP.Core.Domain
{
    public class FollowedProfile
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public string ProfileId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected FollowedProfile() { }

        public FollowedProfile(Guid id, Guid accountId, string profileId)
        {
            Id = id;
            SetAccountId(accountId);
            SetProfileId(profileId);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetAccountId(Guid accountId)
        {
            if (accountId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given Account ID is null.");

            if (accountId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given Account ID is empty.");

            AccountId = accountId;
        }

        private void SetProfileId(string profileId)
        {
            if (profileId == null)
                throw new DomainException(ErrorCodes.ProfileIdIsNull, "Followed profile's ID is null!");

            if (string.IsNullOrWhiteSpace(profileId))
                throw new DomainException(ErrorCodes.ProfileIdIsEmpty, "Followed profile's ID is empty!");

            if (profileId.Length > 128)
                throw new DomainException(ErrorCodes.ProfileIdTooLong, "Followed profile's ID is too long!");

            if (profileId == ProfileId)
                return;

            ProfileId = profileId;
        }
    }
}
