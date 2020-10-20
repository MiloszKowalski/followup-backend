namespace FollowUP.Infrastructure
{
    public static class ErrorCodes
    {
        public static string AccountAlreadyExists => "account_already_exists";
        public static string AccountDoesntExist => "account_doesnt_exist";
        public static string AccountNotAuthenticated => "account_not_authenticated";
        public static string CantGetMedia => "cannot_get_user_media";
        public static string CantGetComments => "cannot_get_media_comments";
        public static string ChallengeRequired => "challenge_required";
        public static string CommentIsEmpty => "comment_is_empty";
        public static string CommentTooLong => "comment_too_long";
        public static string DaysNotPositive => "days_not_positive";
        public static string DuplicatePromotions => "duplicate_promotions";
        public static string EmailInUse => "email_in_use";
        public static string InvalidCredentials => "invalid_credentials";
        public static string InvalidEmail => "invalid_email";
        public static string InvalidMaxIntervalMilliseconds => "invalid_max_interval_milliseconds";
        public static string InvalidMinIntervalMilliseconds => "invalid_min_interval_milliseconds";
        public static string InvalidPromotionType => "invalid_promotion_type";
        public static string InvalidToken => "invalid_token";
        public static string LabelIsEmpty => "label_is_empty";
        public static string LabelTooLong => "label_too_long";
        public static string MaxLessThanMin => "max_less_than_min";
        public static string NoComments => "no_comments";
        public static string NoPromotions => "no_promotions";
        public static string NoProxyAvailable => "no_proxy_available";
        public static string NoScheduleAvailable => "no_schedule_available";
        public static string PromotionNotFound => "promotion_not_found";
        public static string PhoneRequired => "phone_required";
        public static string ServiceUnavailable => "service_unavailable";
        public static string SettingsNotFound => "settings_not_found";
        public static string TextIsEmpty => "text_is_empty";
        public static string TotalPercentageMismatch => "total_percentage_mismatch";
        public static string TwoFactorFailed => "two_factor_failed";
        public static string TwoFactorRequired => "two_factor_required";
        public static string UnknownError => "unknown_error";
        public static string UsernameInUse => "username_in_use";
        public static string UserNotFound => "user_not_found";
        public static string UserNotPermitted => "user_not_permitted";
        public static string VerificationCodeRequired => "verification_code_required";
    }
}