using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Settings;
using System;
using System.IO;
using System.Text;

namespace FollowUP.Infrastructure.Services.Logging
{
    public class InstaActionLogger : IInstaActionLogger
    {
        private readonly ProfileLogLevel _logLevel;
        private readonly bool _logToFile;

        public InstaActionLogger(InstaLoggerSettings settings)
        {
            _logLevel = settings.LogLevel;
            _logToFile = settings.LogToFile;
        }

        public void Log(string message, ProfileLogLevel logLevel, InstagramAccount account)
        {
            if (_logLevel > logLevel)
                return;

            var sb = new StringBuilder();
            sb.Append($"[{DateTime.UtcNow.ToLongTimeString()}]");

            sb.Append($" {message}");
            if (_logToFile)
            {
                LogToFile(sb.ToString(), account);
            }
            else
            {
                Console.WriteLine(sb);
            }
        }

        public void Log(string message, ProfileLogLevel logLevel, InstagramAccount account, IPromotion promotion)
        {
            if (_logLevel > logLevel)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"[{DateTime.UtcNow.ToLongTimeString()}]");

            string promotionName = string.Empty;
            if (promotion == null)
            {
                return;
            }

            if (promotion is HashtagPromotion h)
            {
                promotionName = $"#{h.Label}";
            }
            else if (promotion is ProfilePromotion p)
            {
                promotionName = $"@{p.Label}";
            }
            else if (promotion is UnfollowPromotion)
            {
                promotionName = "Unfollow";
            }

            sb.Append($"({promotionName})");
            sb.Append($" {message}");
            if(_logToFile)
            {
                LogToFile(sb.ToString(), account);
            }
            else
            {
                Console.WriteLine(sb);
            }
        }

        private void LogToFile(string message, InstagramAccount account)
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
                streamWriter.WriteLine(message);
        }
    }
}
