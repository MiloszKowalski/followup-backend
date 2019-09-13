using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Accounts;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class RegisterHandler : ICommandHandler<Register>
    {
        private readonly IUserService _userService;

        public RegisterHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task HandleAsync(Register command)
        {
            await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.Username,
                                    command.FullName, command.Password, Roles.User);
        }
    }
}
