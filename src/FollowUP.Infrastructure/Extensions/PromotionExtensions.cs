using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Extensions
{
    public static class PromotionExtensions
    {
        public static PromotionType GetPromotionType(this IPromotion promotion)
        {
            if (promotion is HashtagPromotion)
            {
                return PromotionType.Hashtag;
            }

            if (promotion is ProfilePromotion)
            {
                return PromotionType.InstagramProfile;
            }

            if (promotion is LocationPromotion)
            {
                return PromotionType.Location;
            }

            if (promotion is UnfollowPromotion)
            {
                return PromotionType.Unfollow;
            }

            return PromotionType.Hashtag;
        }
    }
}
