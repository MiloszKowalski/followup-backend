using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Repositories;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp;
using InstagramApiSharp.Classes;
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
        private readonly IInstagramApiService _instagramApiService;
        private readonly IPromotionService _promotionService;
        private readonly PromotionSettings _settings;
        private readonly SqlSettings _sqlSettings;
        private readonly IMemoryCache _cache;

        public Promoter(IInstagramAccountRepository accountRepository,
                        IInstagramApiService instagramApiService,
                        IPromotionService promotionService, IMemoryCache cache,
                        SqlSettings sqlSettings, PromotionSettings settings)
        {
            _accountRepository = accountRepository;
            _instagramApiService = instagramApiService;
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

            // Get random sleep time for calls' intervals
            var rand = new Random();

            // TODO: Unspaghettify this code ;)
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get all the accounts with promotion module activated
                    var accounts = await _accountRepository.GetAllWithPromotionsAsync();
                    if (!accounts.Any())
                    {
                        Console.WriteLine("Could not find any accounts with promotions module, waiting 10 seconds");
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        continue;
                    }

                    // Night break for it to work 24/7
                    if (DateTime.Now < DateTime.Today.AddHours(_settings.StartingHour) || DateTime.Now > DateTime.Today.AddHours(_settings.EndingHour))
                    {
                        Console.WriteLine($"[{DateTime.Now}] Night break. - ({_settings.EndingHour}:00 - {_settings.StartingHour}:00) - Come back at {_settings.StartingHour} o' clock!");
                        await Task.Delay(TimeSpan.FromHours(1));
                        continue;
                    }

                    // Parallel task for each account with promotion for true asynchrony
                    Parallel.ForEach(accounts, async (account) =>
                    {
                        var task = Task.Run(async () =>
                        {
                            // Skip promotion for the banned account
                            if (account.BannedUntil != null)
                                if (account.BannedUntil > DateTime.UtcNow)
                                {
                                    //Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account banned until {account.BannedUntil}, skipping...");
                                    await Task.Delay(TimeSpan.FromSeconds(5));
                                    return;
                                }

                            // Instantiate new repository for each account to fix async save problem
                            // TODO: Dependency injection with Autofac's proper scopes (per dependecy/scoped?)
                            var promotionRepository = new PromotionRepository(new FollowUPContext(options, _sqlSettings), _settings);
                            var accountRepository = new InstagramAccountRepository(new FollowUPContext(options, _sqlSettings));
                            var statisticsRepository = new StatisticsRepository(new FollowUPContext(options, _sqlSettings));

                            // Get the account's settings to obey the limits
                            var accountSettings = await accountRepository.GetAccountSettingsAsync(account.Id);
                            var statisticsService = new StatisticsService(statisticsRepository);
                            var accountStatistics = await statisticsService.GetTodayAccountStatistics(account.Id);

                            // Get account's statistics
                            if (accountStatistics == null)
                            {
                                var statistics = new AccountStatistics(Guid.NewGuid(), account.Id);
                                await statisticsRepository.AddAsync(statistics);
                                accountStatistics = statistics;
                            }

                            var actionsDone = accountStatistics.ActionsCount;
                            var likesDone = accountStatistics.LikesCount;
                            var followsDone = accountStatistics.FollowsCount;
                            var unFollowsDone = accountStatistics.UnfollowsCount;

                            // Check if we reached the account's limits
                            if (actionsDone >= accountSettings.ActionsPerDay)
                            {
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily action limit! Yay!");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                return;
                            }

                            if (account.ActionCooldown > DateTime.UtcNow)
                            {
                                //Console.WriteLine($"[{DateTime.Now}][{account.Username}] Promotion is on ActionCooldown. Waiting for {(account.ActionCooldown - DateTime.Now).TotalMilliseconds} more milliseconds");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                return;
                            }

                            // Create IInstaApi instance for the given account
                            var instaApi = await _instagramApiService.GetInstaApi(account);
                            if (instaApi == null)
                                return;

                            // Flag to skip unfollow after no profiles found on a certain day.
                            bool canUnfollow = _cache.Get($"{account.Id}-canUnfollow-{DateTime.Today}") != null ? (bool)_cache.Get($"{account.Id}-canUnfollow-{DateTime.Today}") : true;

                            // Random chance to unfollow; if successful, then don't go further
                            // Also, if both likes and follow limits are fulfilled, don't
                            // do unfollow to avoid recurrence (unless specified otherwise in options)
                            if (rand.Next(0, 100) <= _settings.UnfollowChance
                            && ((followsDone < accountSettings.FollowsPerDay
                                || likesDone < accountSettings.LikesPerDay)
                                || _settings.UnfollowAfterPositiveActionsLimits)
                            && canUnfollow)
                            {
                                if (accountStatistics.UnfollowsCount < accountSettings.UnfollowsPerDay)
                                {
                                    var succesfullyUnfollowed = await _promotionService.UnfollowProfile(instaApi, account, promotionRepository, statisticsService, accountRepository, unFollowsDone);
                                    if (succesfullyUnfollowed)
                                    {
                                        // Random chance to look up explore feed for organicity
                                        if (rand.Next(0, 100) <= _settings.ExploreFeedChance)
                                            await _promotionService.LookupExploreFeed(instaApi, account);

                                        // Random chance to look up activity feed for organicity
                                        if (rand.Next(0, 100) <= _settings.ActivityFeedChance)
                                            await _promotionService.LookupActivityFeed(instaApi, account);

                                        return;
                                    }
                                }
                                else
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] There was a challenge required or the account has reached the daily unfollows limit! Yay!");
                            }

                            // Get the current promotion for the given account
                            var promotion = await _promotionService.GetCurrentPromotion(account);
                            if (promotion == null)
                                return;

                            if (account.ActionCooldown != null && account.ActionCooldown > DateTime.UtcNow)
                            {
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Promotion is on ActionCooldown. Waiting for {(account.ActionCooldown - DateTime.Now).TotalMilliseconds} more milliseconds");
                                return;
                            }

                            bool succesfullyFollowed = false;
                            bool succesfullyLiked = false;

                            if (promotion.PromotionType == PromotionType.Hashtag)
                            {
                                // Try getting media list from cache
                                var searchKey = $"{account.Id}-{promotion.Label}-hashtag{_settings.SearchKey}";
                                List<InstaMedia> medias = (List<InstaMedia>)_cache.Get(searchKey);
                                if (medias == null || !medias.Any())
                                {
                                    // If there aren't any medias in cache, update them
                                    var downloadedMedia = await instaApi.GetMediaByHashtagAsync(account, promotion);
                                    _cache.Set(searchKey, downloadedMedia);
                                    return;
                                }

                                // Get random media
                                var lastMediaIndex = medias.Count() - 1;
                                var randomMediaIndex = rand.Next(0, lastMediaIndex);
                                var media = medias[randomMediaIndex];

                                // Set the last media ID for further searching 
                                var takenAt = media.TakenAt;
                                if (takenAt < promotion.NextMinIdDate)
                                {
                                    promotion.SetNextMinIdDate(takenAt);
                                    promotion.SetNextMinId(media.InstaIdentifier);
                                    await promotionRepository.UpdateAsync(promotion);
                                }

                                // Like the media if it hasn't hit the limits
                                if (likesDone < accountSettings.LikesPerDay)
                                {
                                    succesfullyLiked = await _promotionService.LikeMedia(instaApi, account, promotion, promotionRepository, statisticsService, media, likesDone);
                                    if (!succesfullyLiked)
                                        return;
                                }
                                else
                                {
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily likes limit! Yay!");
                                    if (followsDone >= accountSettings.FollowsPerDay)
                                        return;
                                }

                                // Three seconds interval between actions to be more organic
                                await Task.Delay(rand.Next(1000, 4000));

                                // Follow media's author profile if it hasn't hit the limits
                                if (followsDone < accountSettings.FollowsPerDay)
                                {
                                    if (rand.Next(0, 100) <= _settings.FollowChance)
                                    {
                                        succesfullyFollowed = await _promotionService.FollowProfile(instaApi, account, promotion, promotionRepository, statisticsService, accountRepository, media, followsDone);
                                        instaApi.SaveSession();
                                        _cache.Set(account.Id, instaApi);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily follows limit! Yay!");
                                    return;
                                }

                                
                                await _promotionService.SetPromotionCooldown(account, accountRepository);

                                // Remove media from cache
                                medias.Remove(media);

                                // Update remaining media list in cache
                                _cache.Set(searchKey, medias);

                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Current media list count: {medias.Count()}");
                            }
                            else if (promotion.PromotionType == PromotionType.InstagramProfile)
                            {
                                // Try getting users relationships list from cache
                                var searchKey = $"{account.Id}-{promotion.Label}-profile{_settings.SearchKey}";
                                InstaFriendshipShortStatusList followersRelationships = (InstaFriendshipShortStatusList)_cache.Get(searchKey);
                                if (followersRelationships == null || !followersRelationships.Any())
                                {
                                    // If there aren't any users relationships in cache, update them
                                    var downloadedRelationships = await instaApi.GetRelationshipsByPromotionAsync(account, promotion);
                                    _cache.Set(searchKey, downloadedRelationships);
                                    return;
                                }

                                var lastRelationshipIndex = followersRelationships.Count() - 1;
                                var randomRelationshipIndex = rand.Next(0, lastRelationshipIndex);
                                var followerStatus = followersRelationships[randomRelationshipIndex];

                                if (followerStatus.IsPrivate)
                                {
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}](@{promotion.Label}) Account is private, skipping...");
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Current relationship list count: {followersRelationships.Count()}");

                                    // Remove follower status from cache
                                    followersRelationships.Remove(followerStatus);

                                    // Update remaining media list in cache
                                    _cache.Set(searchKey, followersRelationships);
                                    return;
                                }

                                var userMedia = await instaApi.UserProcessor.GetUserMediaByIdAsync(followerStatus.Pk, PaginationParameters.MaxPagesToLoad(1));

                                if (!userMedia.Succeeded)
                                {
                                    if (userMedia.Info.ResponseType == ResponseType.UnExpectedResponse && userMedia.Info.Message != "Not authorized to view user")
                                    {
                                        Console.WriteLine($"[{DateTime.Now}][{account.Username}] Too many actions.");
                                        await _promotionService.SetPromotionCooldown(account, accountRepository, _settings.MinTooManyActionsInterval, _settings.MaxTooManyActionsInterval);
                                    }

                                    return;
                                }

                                if (!userMedia.Value.Any())
                                    return;

                                // Like the media if it hasn't hit the limits
                                if (likesDone < accountSettings.LikesPerDay)
                                {
                                    succesfullyLiked = await _promotionService.LikeMedia(instaApi, account, promotion, promotionRepository, statisticsService, userMedia.Value[0], likesDone);
                                    if (!succesfullyLiked)
                                        return;
                                }
                                else
                                {
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily likes limit! Yay!");
                                    return;
                                }

                                await Task.Delay(rand.Next(1000, 4000));

                                // Follow media's author profile if it hasn't hit the limits
                                if (followsDone < accountSettings.FollowsPerDay)
                                {
                                    if (rand.Next(0, 100) <= _settings.FollowChance)
                                    {
                                        succesfullyFollowed = await _promotionService.FollowProfile(instaApi, account, promotion, promotionRepository, statisticsService, accountRepository, userMedia.Value[0], followsDone);
                                        instaApi.SaveSession();
                                        _cache.Set(account.Id, instaApi);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account has reached the daily follows limit! Yay!");
                                    return;
                                }

                                await _promotionService.SetPromotionCooldown(account, accountRepository);

                                // Remove follower status from cache
                                followersRelationships.Remove(followerStatus);

                                // Update remaining media list in cache
                                _cache.Set(searchKey, followersRelationships);

                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Current relationship list count: {followersRelationships.Count()}");
                            }

                            // Random chance to look up explore feed for organicity
                            if (rand.Next(0, 100) <= _settings.ExploreFeedChance)
                                await _promotionService.LookupExploreFeed(instaApi, account, promotion);

                            // Random chance to look up activity feed for organicity
                            if (rand.Next(0, 100) <= _settings.ActivityFeedChance)
                                await _promotionService.LookupActivityFeed(instaApi, account, promotion);
                        });

                        // Wait for each of the tasks to complete
                        await task;
                    });
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Something went wrong with promotion module (connection error?).");
                    Console.WriteLine(ex.Message);
                    await Task.Delay(5000);
                }
            }
        }
    }
}