namespace FollowUP.Infrastructure.Commands
{
    public class CreateInstagramAccount : AuthenticatedCommandBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string TwoFactorCode { get; set; }
        public string VerificationCode { get; set; }
        public bool PreferSMSVerification { get; set; }
        public bool ReplayChallenge { get; set; }
    }
}
