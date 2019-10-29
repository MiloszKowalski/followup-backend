namespace FollowUP.Infrastructure.Commands
{
    public class LoginToEBInstagram : AuthenticatedCommandBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public string VerificationCode { get; set; }
    }
}
