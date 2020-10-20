using FollowUP.Core.Domain;

namespace FollowUP.Infrastructure.Services.Logging
{
    public static class IInstaActionLoggerExtensions
    {
        public static void LogTrace(this IInstaActionLogger logger, string message, InstagramAccount account)
        {
            logger.Log(message, ProfileLogLevel.Trace, account);
        }

        public static void LogTrace(this IInstaActionLogger logger, string message,
            InstagramAccount account, IPromotion promotion)
        {
            logger.Log(message, ProfileLogLevel.Trace, account, promotion);
        }

        public static void LogInfo(this IInstaActionLogger logger, string message, InstagramAccount account)
        {
            logger.Log(message, ProfileLogLevel.Info, account);
        }

        public static void LogInfo(this IInstaActionLogger logger, string message,
            InstagramAccount account, IPromotion promotion)
        {
            logger.Log(message, ProfileLogLevel.Info, account, promotion);
        }

        public static void LogError(this IInstaActionLogger logger, string message, InstagramAccount account)
        {
            logger.Log(message, ProfileLogLevel.Errors, account);
        }

        public static void LogError(this IInstaActionLogger logger, string message,
            InstagramAccount account, IPromotion promotion)
        {
            logger.Log(message, ProfileLogLevel.Errors, account, promotion);
        }

        public static void LogUser(this IInstaActionLogger logger, string message, InstagramAccount account)
        {
            logger.Log(message, ProfileLogLevel.User, account);
        }

        public static void LogUser(this IInstaActionLogger logger, string message,
            InstagramAccount account, IPromotion promotion)
        {
            logger.Log(message, ProfileLogLevel.User, account, promotion);
        }
    }
}
