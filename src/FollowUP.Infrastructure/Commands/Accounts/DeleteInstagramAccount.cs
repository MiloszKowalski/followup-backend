using System;

namespace FollowUP.Infrastructure.Commands
{
    public class DeleteInstagramAccount : AuthenticatedCommandBase
    {
        public Guid InstagramAccountId { get; set; }
    }
}
