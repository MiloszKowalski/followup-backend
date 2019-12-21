using System;

namespace FollowUP.Core.Domain
{
    public class AccountStatistics
    {
        public Guid Id { get; protected set; }
        public Guid AccountId { get; protected set; }
        public int ActionsCount { get; protected set; }
        public int LikesCount { get; protected set; }
        public int FollowsCount { get; protected set; }
        public int UnfollowsCount { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected AccountStatistics() { }

        public AccountStatistics(Guid id, Guid accountId)
        {
            Id = id;
            SetAccountId(accountId);
            SetActionsCount(0);
            SetLikesCount(0);
            SetFollowsCount(0);
            SetUnfollowsCount(0);
            CreatedAt = DateTime.Today;
        }

        public AccountStatistics(Guid accountId, DateTime createdAt, int actionsCount,
            int likesCount, int followsCount, int unfollowsCount)
        {
            Id = Guid.NewGuid();
            SetAccountId(accountId);
            SetActionsCount(actionsCount);
            SetLikesCount(likesCount);
            SetFollowsCount(followsCount);
            SetUnfollowsCount(unfollowsCount);
            CreatedAt = createdAt;
        }

        public void AddAction()
        {
            ActionsCount++;
        }
        public void AddLike()
        {
            LikesCount++;
        }
        public void AddFollow()
        {
            FollowsCount++;
        }
        public void AddUnfollow()
        {
            UnfollowsCount++;
        }

        private void SetActionsCount(int actionsCount)
        {
            if (actionsCount < 0)
                throw new DomainException(ErrorCodes.NegativeActions, "Actions' count cannot be negative.");

            if (actionsCount == ActionsCount)
                return;

            ActionsCount = actionsCount;
        }

        private void SetLikesCount(int likesCount)
        {
            if (likesCount < 0)
                throw new DomainException(ErrorCodes.NegativeLikes, "Likes' count cannot be negative.");

            if (likesCount == LikesCount)
                return;

            LikesCount = likesCount;
        }

        private void SetFollowsCount(int followsCount)
        {
            if (followsCount < 0)
                throw new DomainException(ErrorCodes.NegativeFollows, "Follows' count cannot be negative.");

            if (followsCount == FollowsCount)
                return;

            FollowsCount = followsCount;
        }

        private void SetUnfollowsCount(int unfollowsCount)
        {
            if (unfollowsCount < 0)
                throw new DomainException(ErrorCodes.NegativeUnfollows, "Unfollows' count cannot be negative.");

            if (unfollowsCount == UnfollowsCount)
                return;

            UnfollowsCount = unfollowsCount;
        }

        private void SetAccountId(Guid accountId)
        {
            if (accountId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given Account ID is null.");

            if (accountId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given Account ID is empty.");

            AccountId = accountId;
        }
    }
}
