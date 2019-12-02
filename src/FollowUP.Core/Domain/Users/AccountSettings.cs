using FollowUP.Domain;
using System;

namespace FollowUP.Core.Domain
{
    public class AccountSettings
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public int ActionsPerDay { get; protected set; }
        public int FollowsPerDay { get; protected set; }
        public int LikesPerDay { get; protected set; }

        protected AccountSettings()
        {
        }

        public AccountSettings(Guid id, Guid accountId, int actionsPerDay = 400,
            int followsPerDay = 250, int likesPerDay = 250)
        {
            Id = id;
            AccountId = accountId;
            ActionsPerDay = actionsPerDay;
            FollowsPerDay = followsPerDay;
            LikesPerDay = likesPerDay;
        }

        public void SetActionsPerDay(int actionsPerDay)
        {
            if (actionsPerDay <= 0)
                throw new DomainException(ErrorCodes.NegativeActions, "Number of actions per day can't be negative.");

            ActionsPerDay = actionsPerDay;
        }

        public void SetLikesPerDay(int likesPerDay)
        {
            if (likesPerDay <= 0)
                throw new DomainException(ErrorCodes.NegativeLikes, "Number of likes per day can't be negative.");

            LikesPerDay = likesPerDay;
        }

        public void SetFollowsPerDay(int followsPerDay)
        {
            if (followsPerDay <= 0)
                throw new DomainException(ErrorCodes.NegativeFollows, "Number of follows can't be negative.");

            FollowsPerDay = followsPerDay;
        }
    }
}
