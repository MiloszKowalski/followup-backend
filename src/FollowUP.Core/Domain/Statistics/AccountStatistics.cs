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
            // TODO: Sanitize the fields
            Id = id;
            AccountId = accountId;
            ActionsCount = 0;
            LikesCount = 0;
            FollowsCount = 0;
            UnfollowsCount = 0;
            CreatedAt = DateTime.Today;
        }

        public AccountStatistics(Guid accountId, DateTime createdAt, int actionsCount,
            int likesCount, int followsCount, int unfollowsCount)
        {
            // TODO: Sanitize the fields
            Id = Guid.NewGuid();
            AccountId = accountId;
            ActionsCount = actionsCount;
            LikesCount = likesCount;
            FollowsCount = followsCount;
            UnfollowsCount = unfollowsCount;
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
    }
}
