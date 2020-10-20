using System;

namespace FollowUP.Core.Domain
{
    public class HashtagPromotion : FollowPromotion
    {
        public HashtagPromotion(Guid id, Guid instagramAccountId,
            string label, DateTime createdAt)
            : base (id, instagramAccountId, label, createdAt)
        {
        }
    }
}
