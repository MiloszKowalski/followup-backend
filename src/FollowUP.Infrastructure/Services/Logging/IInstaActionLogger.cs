using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Services.Logging
{
    public interface IInstaActionLogger
    {
        void Log(string message, ProfileLogLevel logLevel, InstagramAccount account);
        void Log(string message, ProfileLogLevel logLevel, InstagramAccount account, IPromotion promotion);
    }
}
