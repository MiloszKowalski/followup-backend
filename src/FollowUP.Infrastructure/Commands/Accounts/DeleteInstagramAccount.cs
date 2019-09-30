namespace FollowUP.Infrastructure.Commands
{
    public class DeleteInstagramAccount : AuthenticatedCommandBase
    {
        public string Username { get; set; }
    }
}
