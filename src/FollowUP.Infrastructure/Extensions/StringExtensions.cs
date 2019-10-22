namespace FollowUP.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool Empty(this string value)
            => string.IsNullOrWhiteSpace(value);

        public static string Truncate(this string value, int length)
        {
            if (value.Length < length)
                return value;
            return value.Substring(0, length);
        }
    }
}
