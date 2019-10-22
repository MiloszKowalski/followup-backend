using FollowUP.Core.Repositories;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
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

        public CommentsUpdater(IInstagramAccountRepository accountRepository, ICommentService commentService)
        {
            _accountRepository = accountRepository;
            _commentService = commentService;
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

                foreach (var account in accounts)
                {
                    // Set the user's credentials
                    var userSession = new UserSessionData
                    {
                        UserName = account.Username,
                        Password = account.Password
                    };

                    // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
                    var instaApi = InstaApiBuilder.CreateBuilder()
                                                .SetUser(userSession)
                                                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                                                .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                                                .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                                .Build();

                    // Try logging in from session
                    try
                    {
                        instaApi?.SessionHandler?.Load();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not load state from file for user {account.Username}, error info: {e}");
                        Console.WriteLine(e);
                    }

                    Console.WriteLine($"Updating comments for the account: {account.Username}");
                    await _commentService.UpdateAllByAccountId(account.Id);
                }

                Console.WriteLine("Updated comments, waiting 5 minutes");
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}
