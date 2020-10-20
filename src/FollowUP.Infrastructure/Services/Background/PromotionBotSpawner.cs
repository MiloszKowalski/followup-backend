using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Services.Logging;
using FollowUP.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.Background
{
    public class PromotionBotSpawner : BackgroundService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IInstagramApiService _instagramApiService;
        private readonly PromotionSettings _settings;
        private readonly IStatisticsService _statisticsService;
        private readonly IInstaActionLogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IMemoryCache _cache;

        public PromotionBotSpawner(IInstagramAccountRepository accountRepository,
                        IInstagramApiService instagramApiService, IMemoryCache cache,
                        PromotionSettings settings, IStatisticsService statisticsService,
                        IInstaActionLogger logger, IScheduleService scheduleService)
        {
            _accountRepository = accountRepository;
            _instagramApiService = instagramApiService;
            _settings = settings;
            _statisticsService = statisticsService;
            _logger = logger;
            _scheduleService = scheduleService;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Get all the accounts with promotion module activated
            var accounts = await _accountRepository.GetAllWithPromotionsAsync();
            if (!accounts.Any())
            {
                Console.WriteLine("Could not find any accounts with promotions module.");
            }

            Thread thread = null;

            // Spawn a bot for each account
            Parallel.ForEach(accounts, async (account) =>
            {
                // Get the account's settings to obey the limits
                var accountSettings = await _accountRepository.GetAccountSettingsAsync(account.Id);

                // Get account's statistics
                var accountStatistics = await _statisticsService.GetTodayAccountStatisticsAsync(account.Id);
                if (accountStatistics == null)
                {
                    accountStatistics = await _statisticsService.CreateEmptyAsync(account.Id);
                }

                // Get the proper InstaApi instance
                var instaApi = await _instagramApiService.GetInstaApiAsync(account, false);

                // If the user is not authenticated, return
                if (!instaApi.IsUserAuthenticated)
                {
                    _logger.LogError($"The account is not authenticated. The promotion won't start.", account);

                    return;
                }

                // Create new promotion bot and start it on a new thread
                var promotionBot = new PromotionBot(_cache, _settings, _instagramApiService,
                    _logger, instaApi, account, _scheduleService);

                thread = new Thread(new ThreadStart(promotionBot.Promote));
                thread.Start();

                Console.WriteLine($"Started thread for account {account.Username}");
            });
        }
    }
}