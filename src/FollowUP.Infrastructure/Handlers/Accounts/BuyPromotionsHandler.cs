using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class BuyPromotionsHandler : ICommandHandler<BuyPromotions>
    {
        private readonly IInstagramAccountService _accountService;

        public BuyPromotionsHandler(IInstagramAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task HandleAsync(BuyPromotions command)
        {
            await _accountService.BuyPromotionsAsync(command.AccountId, command.DaysToAdd);
        }
    }
}
