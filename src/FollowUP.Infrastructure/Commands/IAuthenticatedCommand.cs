using System;

namespace FollowUP.Infrastructure.Commands
{
    public interface IAuthenticatedCommand : ICommand
    {
        Guid UserId { get; set; }
    }
}
