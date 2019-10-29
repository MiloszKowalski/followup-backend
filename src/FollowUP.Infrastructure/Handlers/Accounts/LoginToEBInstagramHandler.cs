using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class LoginToEBInstagramHandler : ICommandHandler<LoginToEBInstagram>
    {
        private readonly IInstagramAccountService _instagramAccountService;

        public LoginToEBInstagramHandler(IInstagramAccountService instagramAccountService)
        {
            _instagramAccountService = instagramAccountService;
        }

        public async Task HandleAsync(LoginToEBInstagram command)
        {
            await _instagramAccountService.LoginToEmbeddedBrowserAsync(command.Username, command.Password, command.TwoFactorCode, command.VerificationCode);
        }
    }
}
