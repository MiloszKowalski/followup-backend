using FollowUP.Core.Repositories;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.Background
{
    public class CommentsUpdater : BackgroundService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly ICommentService _commentService;
        private readonly IInstagramApiService _instaApiService;

        public CommentsUpdater(IInstagramAccountRepository accountRepository,
            ICommentService commentService, IInstagramApiService instaApiService)
        {
            _accountRepository = accountRepository;
            _commentService = commentService;
            _instaApiService = instaApiService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {              
                var accounts = await _accountRepository.GetAllWithCommentsAsync();

                if (!accounts.Any())
                {
                    Console.WriteLine("Could not find any accounts with comments module, waiting 10 seconds");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    continue;
                }

                Parallel.ForEach(accounts, async (account) =>
                {
                    var instaApi = await _instaApiService.GetInstaApiAsync(account);

                    if(!instaApi.IsUserAuthenticated)
                    {
                        Console.WriteLine($"Could not get comments for the account: {account}. " +
                            $"The account is not authenticated.");
                        return;
                    }

                    Console.WriteLine($"Updating comments for the account: {account.Username}");
                    await _commentService.UpdateAllByAccountIdAsync(account.Id);
                });

                Console.WriteLine("Updated comments, waiting 5 minutes");
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}
