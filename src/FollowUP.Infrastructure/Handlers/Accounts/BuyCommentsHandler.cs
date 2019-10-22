using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class BuyCommentsHandler : ICommandHandler<BuyComments>
    {
        private readonly IInstagramAccountService _accountService;

        public BuyCommentsHandler(IInstagramAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task HandleAsync(BuyComments command)
        {
            await _accountService.BuyComments(command.AccountId, command.DaysToAdd);
        }
    }
}
