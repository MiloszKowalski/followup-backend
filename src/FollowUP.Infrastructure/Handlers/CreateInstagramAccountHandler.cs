using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers
{
    public class CreateInstagramAccountHandler : ICommandHandler<CreateInstagramAccount>
    {
        private readonly IUserService _userService;
        private readonly IInstagramAccountService _instagramAccountService;
        private readonly IMemoryCache _cache;

        public CreateInstagramAccountHandler(IUserService userService,
            IInstagramAccountService instagramAccountService, IMemoryCache cache)
        {
            _userService = userService;
            _instagramAccountService = instagramAccountService;
            _cache = cache;
        }
        public async Task HandleAsync(CreateInstagramAccount command)
        {
            await _instagramAccountService.CreateAsync(Guid.NewGuid(), command.UserId, command.Username, command.Password);
        }
    }
}
