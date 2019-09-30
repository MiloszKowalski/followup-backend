using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class LoginToInstagramHandler : ICommandHandler<LoginToInstagram>
    {
        private readonly IInstagramAccountService _instagramAccountService;
        private readonly IInstagramAccountRepository _instagramAccountRepository;

        public LoginToInstagramHandler(IInstagramAccountService instagramAccountService,
                                        IInstagramAccountRepository instagramAccountRepository)
        {
            _instagramAccountService = instagramAccountService;
            _instagramAccountRepository = instagramAccountRepository;
        }

        public async Task HandleAsync(LoginToInstagram command)
        {
            var accountId = Guid.NewGuid();
            await _instagramAccountService.CreateAsync(accountId, command.UserId, command.Username, command.Password);
            await _instagramAccountService.LoginAsync(command.Username, command.Password, command.PhoneNumber, command.TwoFactorCode,
                                                      command.VerificationCode, command.PreferSMSVerification, command.ReplayChallenge);
        }
    }
}
