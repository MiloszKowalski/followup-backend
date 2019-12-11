namespace FollowUP.Domain
{
    public static class ErrorCodes
    {
        public static string InvalidEmail => "invalid_email";
        public static string InvalidFullName => "invalid_fullname";
        public static string InvalidGuid => "invalid_guid";
        public static string InvalidPassword => "invalid_password";
        public static string InvalidRole => "invalid_role";
        public static string InvalidUsername => "invalid_username";
        public static string NegativeLikes => "negative_likes";
        public static string NegativeActions => "negative_actions";
        public static string NegativeFollows => "negative_follows";
        public static string NegativeUnfollows => "negative_unfollows";
    }
}