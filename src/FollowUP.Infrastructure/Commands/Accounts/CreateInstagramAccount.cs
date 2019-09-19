namespace FollowUP.Infrastructure.Commands
{
    public class CreateInstagramAccount : AuthenticatedCommandBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
