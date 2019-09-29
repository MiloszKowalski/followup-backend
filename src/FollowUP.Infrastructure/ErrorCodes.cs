namespace FollowUP.Infrastructure
{
    public static class ErrorCodes
    {
        public static string AccountAlreadyExists => "account_already_exists";
        public static string AccountDoesntExist => "account_doesnt_exist";
        public static string AccountNotAuthenticated => "account_not_authenticated";
        public static string CantGetMedia => "cannot_get_user_media";
        public static string CantGetComments => "cannot_get_media_comments";
        public static string EmailInUse => "email_in_use";
        public static string InvalidCredentials => "invalid_credentials";
        public static string InvalidEmail => "invalid_email";
        public static string UsernameInUse => "username_in_use";
        public static string UserNotFound => "user_not_found";
    }
}