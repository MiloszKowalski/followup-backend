namespace FollowUP.Infrastructure
{
    public static class ErrorCodes
    {
        public static string AccountAlreadyExists => "account_already_exists";
        public static string AccountDoesntExist => "account_doesnt_exist";
        public static string AccountNotAuthenticated => "account_not_authenticated";
        public static string CantGetMedia => "cannot_get_user_media";
        public static string CantGetComments => "cannot_get_media_comments";
        public static string CommentIsEmpty => "comment_is_empty";
        public static string CommentTooLong => "comment_too_long";
        public static string DaysNotPositive => "days_not_positive";
        public static string EmailInUse => "email_in_use";
        public static string InvalidCredentials => "invalid_credentials";
        public static string InvalidEmail => "invalid_email";
        public static string InvalidPromotionType => "invalid_promotion_type";
        public static string InvalidToken => "invalid_token";
        public static string LabelIsEmpty => "label_is_empty";
        public static string LabelTooLong => "label_too_long";
        public static string NoComments => "no_comments";
        public static string NoPromotions => "no_promotions";
        public static string NoProxyAvailable => "no_proxy_available";
        public static string NoScheduleAvailable => "no_schedule_available";
        public static string TotalPercentageMismatch => "total_percentage_mismatch";
        public static string UsernameInUse => "username_in_use";
        public static string UserNotFound => "user_not_found";
        public static string UserNotPermitted => "user_not_permitted";
    }
}