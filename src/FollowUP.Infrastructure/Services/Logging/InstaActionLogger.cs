using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Settings;
using System;
using System.IO;
using System.Text;

namespace FollowUP.Infrastructure.Services.Logging
{
    public class InstaActionLogger : IInstaActionLogger
    {
        private readonly InstaLogLevel _logLevel;
        private readonly bool _logToFile;

        public InstaActionLogger(InstaLoggerSettings settings)
        {
            _logLevel = settings.LogLevel;
            _logToFile = settings.LogToFile;
        }

        public void Log(string message, InstaLogLevel logLevel, InstagramAccount account, Promotion promotion = null)
        {
            if (_logLevel > logLevel)
                return;

            var sb = new StringBuilder();
            sb.Append($"[{DateTime.UtcNow.ToLongTimeString()}]");

            string promotionName;
            if(promotion != null)
            {
                if (promotion.PromotionType == PromotionType.Hashtag)
                    promotionName = $"#{promotion.Label}";
                else
                    promotionName = $"@{promotion.Label}";
            }
            else
                promotionName = "Unfollow";

            sb.Append($"({promotionName})");
            sb.Append($" {message}");
            if(_logToFile)
            {
                string accountPath = account.FilePath.Replace('/', Path.DirectorySeparatorChar)
                                                     .Replace('\\', Path.DirectorySeparatorChar);
                string[] accountDir = accountPath.Split(Path.DirectorySeparatorChar);

                string logFilePath = Path.Combine(accountDir[0], accountDir[1],
                                     "logs", account.Username.ToString());

                if (!Directory.Exists(logFilePath))
                    Directory.CreateDirectory(logFilePath);

                string todayPath = Path.Combine(logFilePath, DateTime.Today.ToLongDateString() + ".log");

                using (var streamWriter = File.AppendText(todayPath))
                    streamWriter.WriteLine(sb);
            }
            else
            {
                Console.WriteLine(sb);
            }
        }
    }
}
