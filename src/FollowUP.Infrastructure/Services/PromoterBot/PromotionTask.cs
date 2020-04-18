using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Services
{
    public class PromotionTask
    {
        public Promotion Promotion { get; set; }
        public int MillisecondsToWaitAfterDone { get; set; }

        public PromotionTask(Promotion promotion, int millisecondsToWaitAfterDone)
        {
            Promotion = promotion;
            MillisecondsToWaitAfterDone = millisecondsToWaitAfterDone;
        }
    }
}
