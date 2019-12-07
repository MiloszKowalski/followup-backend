using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Repositories;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp.Classes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.Background
{
    public class Promoter : BackgroundService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IPromotionService _promotionService;
        private readonly PromotionSettings _settings;
        private readonly SqlSettings _sqlSettings;
        private readonly IMemoryCache _cache;

        public Promoter(IInstagramAccountRepository accountRepository, IPromotionService promotionService,
            IMemoryCache cache, SqlSettings sqlSettings, PromotionSettings settings)
        {
            _accountRepository = accountRepository;
            _promotionService = promotionService;
            _sqlSettings = sqlSettings;
            _settings = settings;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait 5 secs for server to warm up and do the DI stuff
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Options for the promotion repository instances
            var options = new DbContextOptionsBuilder<FollowUPContext>()
                .UseSqlServer(_sqlSettings.ConnectionString)
                .Options;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Get all the accounts with promotion module activated
                var accounts = await _accountRepository.GetAllWithPromotionsAsync();
                if (!accounts.Any())
                {
                    Console.WriteLine("Could not find any accounts with promotions module, waiting 10 seconds");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    continue;
                }

                // Get random sleep time for calls' intervals
                var rand = new Random();

                // Parallel task for each account with promotion for true asynchrony
                Parallel.ForEach(accounts, (account) =>
                 {
                    var task = Task.Run(async () =>
                    {
                        // Skip promotion for the banned account
                        if(account.BannedUntil != null)
                            if (account.BannedUntil > DateTime.UtcNow)
                            {
                                //Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account banned until {account.BannedUntil}, skipping...");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                return;
                            }

                        // Get the account's settings to obey the limits
                        var accountSettings = await _accountRepository.GetAccountSettingsAsync(account.Id);

                        // Meantime to check how many follows, unfollows, likes were made by cache
                        _cache.TryGetValue($"{account.Id}-follows-count", out int followsDone);
                        _cache.TryGetValue($"{account.Id}-unfollows-count", out int unFollowsDone);
                        _cache.TryGetValue($"{account.Id}-likes-count", out int likesDone);

                        // Check if we reached the account's limits
                        if(followsDone + unFollowsDone + likesDone >= accountSettings.ActionsPerDay)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily action limit! Yay!");
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            return;
                        }

                        // Instantiate new repository for each account to fix async save problem
                        var _promotionRepository = new PromotionRepository(new FollowUPContext(options, _sqlSettings), _settings);

                        // Create IInstaApi instance for the given account
                        var instaApi = await _promotionService.GetInstaApi(account);
                        if (instaApi == null)
                            return;

                        // Get the current promotion for the given account
                        var promotion = await _promotionService.GetCurrentPromotion(account);
                        if (promotion == null)
                            return;

                        if (promotion.ActionCooldown != null && promotion.ActionCooldown > DateTime.UtcNow)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Promotion is on ActionCooldown. Waiting for {(promotion.ActionCooldown - DateTime.Now).TotalMilliseconds} more milliseconds");
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            return;
                        }
                        
                        if (promotion.PromotionType == PromotionType.Hashtag)
                        {
                            // Try getting media list from cache
                            var searchKey = $"{account.Id}-{promotion.Label}-hashtag{_settings.SearchKey}";
                            List<InstaMedia> medias = (List<InstaMedia>)_cache.Get(searchKey);
                            if(medias == null || !medias.Any())
                            {
                                // If there aren't any medias in cache, update them
                                var downloadedMedia = await instaApi.GetMediaByHashtagAsync(account, promotion, _promotionRepository);
                                _cache.Set(searchKey, downloadedMedia);
                                return;
                            }

                            // Set the last media ID for further searching
                            var lastMediaIndex = medias.Count() - 1;
                            var lastMedia = medias[lastMediaIndex];
                            promotion.SetNexMinId(lastMedia.InstaIdentifier);

                            var randomMediaIndex = rand.Next(0, lastMediaIndex);
                            var media = medias[randomMediaIndex];

                            // Random chance to unfollow; if successful, then don't go further
                            if (rand.Next(0, 100) > 50)
                            {
                                var succesfullyFollowed = await _promotionService.UnfollowProfile(instaApi, account, promotion, _promotionRepository, unFollowsDone); ;
                                if(succesfullyFollowed)
                                    return;
                            }

                            // Like the media if it hasn't hit the limits
                            if(likesDone < accountSettings.LikesPerDay)
                                await _promotionService.LikeMedia(instaApi, account, promotion, _promotionRepository, media, likesDone);
                            else
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily likes limit! Yay!");

                            // Three seconds interval between actions to be more organic
                            await Task.Delay(TimeSpan.FromSeconds(3));

                            // Follow media's author profile if it hasn't hit the limits
                            if(followsDone < accountSettings.FollowsPerDay)
                                await _promotionService.FollowProfile(instaApi, account, promotion, _promotionRepository, media, followsDone);
                            else
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily follows limit! Yay!");

                            // Random chance to look up explore feed for organicity
                            if (rand.Next(0, 100) > 75)
                                await _promotionService.LookupExploreFeed(instaApi, account, promotion);

                            // Random chance to look up activity feed for organicity
                            if(rand.Next(0, 100) > 75)
                                await _promotionService.LookupActivityFeed(instaApi, account, promotion);

                            // Add the media to the blacklist
                            var blackListMedia = new CompletedMedia(Guid.NewGuid(), account.Id, media.Code, DateTime.UtcNow);
                            await _promotionRepository.AddToBlacklistAsync(blackListMedia);

                            // Remove media from cache
                            medias.Remove(media);

                            // Update remaining media list in cache
                            _cache.Set(searchKey, medias);

                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Current media list count: {medias.Count()}");
                        }
                    });

                    // Wait for each of the tasks to complete
                    task.Wait();
                });
                Console.WriteLine("-------------------------------------------------------------------");
            }
        }
    }
}