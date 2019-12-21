namespace FollowUP.Core.Domain
{
    public static class ErrorCodes
    {
        public static string AuthorIsEmpty => "author_is_empty";
        public static string AuthorIsNull => "author_is_null";
        public static string AuthorTooLong => "author_too_long";
        public static string ContentIsEmpty => "content_is_empty";
        public static string ContentIsNull => "content_is_null";
        public static string ContentTooLong => "content_too_long";
        public static string DateFromPast => "date_from_past";
        public static string EnumOutOfBounds => "enum_out_of_bounds";
        public static string GuidIsEmpty => "guid_is_empty";
        public static string GuidIsNull => "guid_is_null";
        public static string ImageUriIsEmpty => "image_uri_is_empty";
        public static string ImageUriIsNull => "image_uri_is_null";
        public static string ImageUriTooLong => "image_uri_too_long";
        public static string InvalidAuthorId => "invalid_author_id";
        public static string InvalidDevice => "invalid_device";
        public static string InvalidEmail => "invalid_email";
        public static string InvalidFullName => "invalid_fullname";
        public static string InvalidGuid => "invalid_guid";
        public static string InvalidPassword => "invalid_password";
        public static string InvalidProxy => "invalid_proxy";
        public static string InvalidRole => "invalid_role";
        public static string InvalidUsername => "invalid_username";
        public static string LabelIsEmpty => "label_is_empty";
        public static string LabelIsNull => "label_is_null";
        public static string LabelTooLong => "label_too_long";
        public static string MediaIdIsEmpty => "media_id_is_empty";
        public static string MediaIdIsNull => "media_id_is_null";
        public static string MediaIdTooLong => "media_id_too_long";
        public static string NegativeLikes => "negative_likes";
        public static string NegativeActions => "negative_actions";
        public static string NegativeDays => "negative_days";
        public static string NegativeEnum => "negative_enum";
        public static string NegativeFollows => "negative_follows";
        public static string NegativeUnfollows => "negative_unfollows";
        public static string PasswordIsEmpty => "password_is_empty";
        public static string PasswordIsNull => "password_is_null";
        public static string PasswordTooLong => "password_too_long";
        public static string ProfileIdIsEmpty => "profile_id_is_empty";
        public static string ProfileIdIsNull => "profile_id_is_null";
        public static string ProfileIdTooLong => "profile_id_too_long";
        public static string ProfilePictureUriIsEmpty => "profile_picture_uri_is_empty";
        public static string ProfilePictureUriIsNull => "profile_picture_uri_is_null";
        public static string UsernameIsEmpty => "username_is_empty";
        public static string UsernameIsNull => "username_is_null";
        public static string UsernameTooLong => "username_too_long";
    }
}