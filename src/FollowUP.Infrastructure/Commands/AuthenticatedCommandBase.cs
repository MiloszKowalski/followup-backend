using System;

namespace FollowUP.Infrastructure.Commands
{
    public class AuthenticatedCommandBase : IAuthenticatedCommand
    {
        public Guid UserId { get; set; }
    }
}
