using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Services
{
    /// <summary>
    /// A single task to perform containing <see cref="Promotion"/>
    /// and time to wait after performing it in milliseconds
    /// </summary>
    public class PromotionTask
    {
        public IPromotion Promotion { get; set; }
        public int MillisecondsToWaitAfterDone { get; set; }

        public PromotionTask(IPromotion promotion, int millisecondsToWaitAfterDone)
        {
            Promotion = promotion;
            MillisecondsToWaitAfterDone = millisecondsToWaitAfterDone;
        }
    }
}
