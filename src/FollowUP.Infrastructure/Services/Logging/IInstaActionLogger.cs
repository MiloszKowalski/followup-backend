using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Services.Logging
{
    public interface IInstaActionLogger
    {
        void Log(string message, InstaLogLevel logLevel, InstagramAccount account);
        void Log(string message, InstaLogLevel logLevel, InstagramAccount account, Promotion promotion = null);
    }
}
