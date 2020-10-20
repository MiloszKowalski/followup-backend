﻿using System;

namespace FollowUP.Core.Domain
{
    public class LocationPromotion : FollowPromotion
    {
        public LocationPromotion(Guid id, Guid instagramAccountId,
            string label, DateTime createdAt)
            : base(id, instagramAccountId, label, createdAt)
        {
        }
    }
}
