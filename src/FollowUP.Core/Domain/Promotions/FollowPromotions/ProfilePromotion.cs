using System;

namespace FollowUP.Core.Domain
{
    public class ProfilePromotion : FollowPromotion
    {
        public ProfilePromotion(Guid id, Guid instagramAccountId,
            string label, DateTime createdAt)
            : base(id, instagramAccountId, label, createdAt)
        {
        }
    }
}
