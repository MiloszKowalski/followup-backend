using System;

namespace FollowUP.Core.Domain
{
    public class AccountSettings
    {
        public Guid Id { get; protected set; }

        public int ActionsPerDay { get; protected set; }
        public int FollowsPerDay { get; protected set; }
        public int LikesPerDay { get; protected set; }
        public int MinIntervalMilliseconds { get; set; }
        public int MaxIntervalMilliseconds { get; set; }
        public int UnfollowsPerDay { get; protected set; }

        public Guid InstagramAccountId { get; set; }
        public InstagramAccount InstagramAccount { get; set; }

        protected AccountSettings()
        {
        }

        public AccountSettings(Guid id, Guid accountId, int actionsPerDay = 500,
            int followsPerDay = 250, int likesPerDay = 250, int unfollowsPerDay = 250,
            int minIntervalMilliseconds = 0, int maxIntervalMilliseconds = 0)
        {
            Id = id;
            InstagramAccountId = accountId;
            SetActionsPerDay(actionsPerDay);
            SetFollowsPerDay(followsPerDay);
            SetUnfollowsPerDay(unfollowsPerDay);
            SetLikesPerDay(likesPerDay);
            SetMaxIntervalMilliseconds(maxIntervalMilliseconds);
            SetMinIntervalMilliseconds(minIntervalMilliseconds);
        }

        public void SetActionsPerDay(int actionsPerDay)
        {
            if (actionsPerDay <= 0)
            {
                throw new DomainException(ErrorCodes.NegativeActions,
                    "Number of actions per day can't be negative.");
            }

            ActionsPerDay = actionsPerDay;
        }

        public void SetLikesPerDay(int likesPerDay)
        {
            if (likesPerDay <= 0)
            {
                throw new DomainException(ErrorCodes.NegativeLikes,
                    "Number of likes per day can't be negative.");
            }

            LikesPerDay = likesPerDay;
        }

        public void SetFollowsPerDay(int followsPerDay)
        {
            if (followsPerDay <= 0)
            {
                throw new DomainException(ErrorCodes.NegativeFollows,
                    "Number of follows can't be negative.");
            }

            FollowsPerDay = followsPerDay;
        }

        public void SetUnfollowsPerDay(int unfollowsPerDay)
        {
            if (unfollowsPerDay <= 0)
            {
                throw new DomainException(ErrorCodes.NegativeUnfollows,
                    "Number of unfollows can't be negative.");
            }

            UnfollowsPerDay = unfollowsPerDay;
        }

        public void SetMaxIntervalMilliseconds(int maxIntervalMilliseconds)
        {
            if (maxIntervalMilliseconds == MaxIntervalMilliseconds)
            {
                return;
            }

            if (maxIntervalMilliseconds < 0)
            {
                throw new DomainException(ErrorCodes.NegativeValue,
                    $"Number of max interval milliseconds can't be negative.");
            }

            if (maxIntervalMilliseconds < MinIntervalMilliseconds)
            {
                throw new DomainException(ErrorCodes.MaxSmallerThanMin,
                    $"Maximum of interval milliseconds can't be smaller than its minimum.");
            }

            MaxIntervalMilliseconds = maxIntervalMilliseconds;
        }

        public void SetMinIntervalMilliseconds(int minIntervalMilliseconds)
        {
            if (MinIntervalMilliseconds == minIntervalMilliseconds)
            {
                return;
            }

            if (minIntervalMilliseconds < 0)
            {
                throw new DomainException(ErrorCodes.NegativeValue,
                    $"Number of min interval milliseconds can't be negative.");
            }

            if (minIntervalMilliseconds > MaxIntervalMilliseconds)
            {
                throw new DomainException(ErrorCodes.MinGreaterThanMax,
                    $"Minimum of interval milliseconds can't be greater than its maximum.");
            }

            MinIntervalMilliseconds = minIntervalMilliseconds;
        }
    }
}
