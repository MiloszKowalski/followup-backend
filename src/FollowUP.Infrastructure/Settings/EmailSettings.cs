namespace FollowUP.Infrastructure.Settings
{
    public class EmailSettings
    {
        public bool SendVerificationMail { get; set; }
        public string SendGridKey { get; set; }
        public string SendFromEmail { get; set; }
        public string SendFromName { get; set; }
    }
}
