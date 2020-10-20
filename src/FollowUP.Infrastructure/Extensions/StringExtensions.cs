using System;
using System.Text;

namespace FollowUP.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool Empty(this string value)
            => string.IsNullOrWhiteSpace(value);

        public static string Truncate(this string value, int length)
        {
            if (value.Length < length)
            {
                return value;
            }

            return value.Substring(0, length);
        }

        public static string EncodeToBase64String(this string decoded)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(decoded));
        }

        public static string DecodeFromBase64String(this string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }
    }
}
