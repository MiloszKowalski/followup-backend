using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Users;
using FollowUP.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Users
{
    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IUserService _userService;

        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task HandleAsync(CreateUser command)
        {
            await _userService.RegisterAsync(Guid.NewGuid(), command.Email,
                command.Username, command.Password, command.Role);
        }
    }
}
