using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class LoginToInstagramHandler : ICommandHandler<LoginToInstagram>
    {
        private readonly IInstagramAccountService _instagramAccountService;
        private readonly IMemoryCache _cache;

        public LoginToInstagramHandler(IInstagramAccountService instagramAccountService, IMemoryCache cache)
        {
            _instagramAccountService = instagramAccountService;
            _cache = cache;
        }

        public async Task HandleAsync(LoginToInstagram command)
        {
                await _instagramAccountService.LoginAsync(command.Username, command.Password, command.PhoneNumber, command.TwoFactorCode,
                                                      command.VerificationCode, command.PreferSMSVerification, command.ReplayChallenge);
        }
    }
}
