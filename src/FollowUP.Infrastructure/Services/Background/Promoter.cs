using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Repositories;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.Background
{
    public class Promoter : BackgroundService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly IMemoryCache _cache;
        private readonly SqlSettings _sqlSettings;
        private readonly PromotionSettings _settings;

        public Promoter(IInstagramAccountRepository accountRepository, IProxyRepository proxyRepository,
            IMemoryCache cache, SqlSettings sqlSettings, PromotionSettings settings)
        {
            _accountRepository = accountRepository;
            _proxyRepository = proxyRepository;
            _cache = cache;
            _sqlSettings = sqlSettings;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait 10 secs to load
            await Task.Delay(10000);

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

                Parallel.ForEach(accounts, (account) =>
                {
                    var task = Task.Run(async () =>
                    {
                        if(account.BannedUntil != null)
                            if (account.BannedUntil > DateTime.UtcNow)
                            {
                                //Console.WriteLine($"[{DateTime.Now}][{account.Username}] Account banned until {account.BannedUntil}, skipping...");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                return;
                            }
                        var _promotionRepository = new PromotionRepository(new FollowUPContext(options, _sqlSettings));
                        // Divide proxy to proper proxy parts
                        var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);

                        if (accountProxy == null)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Can't find any working proxy, skipping...");
                            await Task.Delay(5000);
                            return;
                        }

                        var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

                        if (proxyInfo.ExpiryDate < DateTime.UtcNow)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Proxy {proxyInfo.Ip} is expired, skipping...");
                            await Task.Delay(5000);
                            return;
                        }

                        var proxy = new InstaProxy(proxyInfo.Ip, proxyInfo.Port)
                        {
                            Credentials = new NetworkCredential(proxyInfo.Username, proxyInfo.Password)
                        };

                        // Now create a client handler which uses that proxy
                        var httpClientHandler = new HttpClientHandler()
                        {
                            Proxy = proxy,
                        };

                        // Set the user's credentials
                        var userSession = new UserSessionData
                        {
                            UserName = account.Username,
                            Password = account.Password
                        };

                        // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
                        var instaApi = InstaApiBuilder.CreateBuilder()
                                                    .SetUser(userSession)
                                                    .UseLogger(new DebugLogger(InstagramApiSharp.Logger.LogLevel.Exceptions))
                                                    .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                                                    .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                                    .UseHttpClientHandler(httpClientHandler)
                                                    .Build();

                        // Check if there is an instaApi instance bound to the account...
                        var instaApiCache = (IInstaApi)_cache.Get(account.Id);

                        if (instaApiCache != null)
                        {
                            // ...if true, use it
                            instaApi = instaApiCache;
                        }


                        // TODO: Device service
                        instaApi.SetApiVersion(InstaApiVersionType.Version117);
                        instaApi.SetDevice(AndroidDeviceGenerator.GetByName(AndroidDevices.XIAOMI_REDMI_NOTE_4X));

                        // Try logging in from session
                        try
                        {
                            instaApi?.SessionHandler?.Load();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Could not load state from file, error info: {e.Message}");
                            await Task.Delay(10000);
                            return;
                        }

                        if(!instaApi.IsUserAuthenticated)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] User not logged in, please authenticate first.");
                            await Task.Delay(1000);
                            return;
                        }

                        // Get all account's promotions
                        string promotionKey = $"{account.Id}{_settings.PromotionKey}";
                        var promotions = await _promotionRepository.GetAccountPromotionsAsync(account.Id);

                        Console.WriteLine();

                        if (promotions == null || !promotions.Any())
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Couldn't find any promotions, skipping");
                            await Task.Delay(5000);
                            return;
                        }

                        await Task.Delay(3000);

                        // Queue the promotions to make only one per iteration
                        var currentPromotion = promotions.First();

                        var previousPromotion = (Promotion)_cache.Get(promotionKey);

                        if (previousPromotion != null)
                        {
                            var promotionList = promotions.ToList();

                            int previousPromotionIndex = -1;
                            foreach(var promo in promotionList)
                            {
                                if (promo.Id == previousPromotion.Id)
                                {
                                    previousPromotionIndex = promotionList.IndexOf(promo);
                                    break;
                                }
                            }
                            if (previousPromotionIndex > -1 && promotionList[previousPromotionIndex].ActionCooldown > DateTime.UtcNow)
                            {
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Promotion is on ActionCooldown. Waiting for {(promotionList[previousPromotionIndex].ActionCooldown - DateTime.Now).TotalMilliseconds} more milliseconds");
                                return;
                            }
                            currentPromotion = promotionList[previousPromotionIndex >= promotionList.Count - 1 ? 0 : (previousPromotionIndex + 1)];
                            var timeDifference = (int)previousPromotion.ActionCooldown.Subtract(DateTime.UtcNow).TotalMilliseconds;
                            if(timeDifference > 0)
                                currentPromotion.SetActionCooldown(timeDifference);
                        }
                        _cache.Set(promotionKey, currentPromotion);

                        // Get all account's comments templates
                        var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(account.Id);
                        var promotion = currentPromotion;

                        if (promotion.ActionCooldown != null && promotion.ActionCooldown > DateTime.UtcNow)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Promotion is on ActionCooldown. Waiting for {(promotion.ActionCooldown - DateTime.Now).TotalMilliseconds} more milliseconds");
                            await Task.Delay(5000);
                            return;
                        }
                        
                        if (promotion.PromotionType == PromotionType.Hashtag)
                        {
                            // Try getting media list from cache
                            var searchKey = $"{account.Id}_{promotion.Label}";
                            List<InstaMedia> medias = (List<InstaMedia>)_cache.Get(searchKey);
                            if(medias == null || !medias.Any())
                            {
                                // If there aren't any medias in cache, update them
                                var downloadedMedia = await GetMediaByHashtagAsync(instaApi, account, promotion, _promotionRepository);
                                _cache.Set(searchKey, downloadedMedia);
                                return;
                            }

                            var mediasToRemove = new List<InstaMedia>();
                            bool completed = false;
                            var randomMediaIndex = rand.Next(0, medias.Count() - 1);
                            var randomMedia = medias[randomMediaIndex];
                            foreach (var media in medias)
                            {
                                if (completed)
                                    break;
                                if (media != randomMedia)
                                    continue;
                                var mediaCheck = await _promotionRepository.GetMediaAsync(media.Code, account.Id);
                                if (mediaCheck != null)
                                    if (media.Code == mediaCheck.Code && mediaCheck.AccountId == account.Id)
                                    {
                                        mediasToRemove.Add(media);
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) Skipped media {media?.Caption?.Text?.ToString().Truncate(20)}");
                                        continue;
                                    }

                                var followResponse = await instaApi.UserProcessor.FollowUserAsync(media.User.Pk);
                                if (followResponse.Succeeded)
                                    Console.WriteLine($"[{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Success!");
                                else
                                {
                                    Console.WriteLine($"[{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Failed: {followResponse.Info.Message} - {followResponse.Info.ResponseType}");
                                    if (followResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                                    {
                                        Console.WriteLine("Turning off promotion...");

                                        // HACK: As the database operations weren't thread safe, it occasionally would crash
                                        // when two threads were saving at the same time. So we repeat this operation
                                        // until it no longer crashes

                                        bool saveSuccessful = false;

                                        while (!saveSuccessful)
                                        {
                                            try
                                            {
                                                account.BannedUntil = DateTime.UtcNow.AddDays(_settings.BanDurationInDays);
                                                account.PromotionsModuleExpiry = account.PromotionsModuleExpiry.AddDays(_settings.BanDurationInDays);
                                                await _accountRepository.UpdateAsync(account);
                                                saveSuccessful = true;
                                                return;
                                            }
                                            catch
                                            {
                                                saveSuccessful = false;
                                                Console.WriteLine("Turning off promotion failed. Retrying...");
                                            }
                                        }
                                    }
                                }

                                if (rand.Next(0, 100) > 75)
                                {
                                    await LookupExploreFeed(instaApi, account, promotion);
                                }

                                if(rand.Next(0, 100) > 75)
                                {
                                    await LookupActivityFeed(instaApi, account, promotion);
                                }


                                var blackListMedia = new CompletedMedia(Guid.NewGuid(), account.Id, media.Code, DateTime.UtcNow);

                                mediasToRemove.Add(media);

                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Current media list count: {medias.Count()}");

                                promotion.SetNexMinId(media.InstaIdentifier);

                                var milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
                                var previousMilliseconds = promotion.PreviousCooldownMilliseconds;
                                // If the difference between this interval and the previous
                                // one is less than 30 seconds, randomize interval again
                                while (Math.Abs(previousMilliseconds - milliseconds) < _settings.MinIntervalDifference)
                                    milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
                                promotion.SetActionCooldown(milliseconds);

                                await _promotionRepository.UpdateAsync(promotion);
                                await _promotionRepository.AddToBlacklistAsync(blackListMedia);
                                    
                                completed = true;
                                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Waiting {milliseconds} milliseconds");
                            }
                                
                            // Remove media from cache
                            foreach (var media in mediasToRemove)
                                medias.Remove(media);

                            _cache.Set(searchKey, medias);
                        }
                    });

                    // Wait for each of the tasks to complete
                    task.Wait();
                });

                Console.WriteLine("-------------------------------------------------------------------");
            }
        }

        private async Task<List<InstaMedia>> GetMediaByHashtagAsync(IInstaApi instaApi, InstagramAccount account, Promotion promotion, IPromotionRepository promotionRepository)
        {
            int firstHashtagCount = 0;

            var firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            if (firstHashtagResponse.Info.Message == "challenge_required")
            {
                await instaApi.GetLoggedInChallengeDataInfoAsync();
                await instaApi.AcceptChallengeAsync();
                firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            }
            
            if (!firstHashtagResponse.Succeeded)
                return new List<InstaMedia>();

            var firstMedia = firstHashtagResponse.Value.Medias;

            
            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Getting media by tag {promotion.Label}...");

            foreach (var media in firstMedia)
            {
                var blackMedia = await promotionRepository.GetMediaAsync(media.Code, account.Id);

                if (media.Code == blackMedia?.Code && blackMedia?.AccountId == account.Id)
                {
                    firstHashtagCount++;
                }
            }

            if (firstMedia.Count > firstHashtagCount)
                return firstMedia;

            var pagination = PaginationParameters.Empty;
            pagination.StartFromMinId(promotion.NextMinId);

            var hashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, pagination);

            if (!hashtagResponse.Succeeded)
                return new List<InstaMedia>();

            return hashtagResponse.Value.Medias;
        }

        private async Task<bool> LookupActivityFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion)
        {
            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - Start");

            var rand = new Random();
            string activityKey = $"{account.Id}-activity";
            var lastActivity = (List<InstaRecentActivityFeed>)_cache.Get(activityKey);

            if (lastActivity == null)
            {
                lastActivity = new List<InstaRecentActivityFeed>();
            }

            var activity = await instaApi.FeedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            if (!activity.Succeeded)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - Getting recent activity - Error: {activity.Info.ResponseType} - {activity.Info.Message}");
                return false;
            }

            var activityNotifications = activity.Value?.Items;
            if (activityNotifications.Count <= 0 || activityNotifications.Count <= lastActivity.Count)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - No new posts");
                return false;
            }

            _cache.Set(activityKey, activityNotifications);

            var firstActivity = activityNotifications[0];
            if (firstActivity == null)
                return false;

            if (firstActivity.Links == null)
                return false;

            foreach (var link in firstActivity.Links)
            {
                if (link.Type != "user")
                    continue;
                
                long.TryParse(link.Id, out long userId);
                var userMedia = await instaApi.UserProcessor.GetUserMediaByIdAsync(userId, PaginationParameters.MaxPagesToLoad(1));
                if (!userMedia.Succeeded)
                    continue;

                if (userMedia == null)
                    continue;
                
                if (userMedia.Value.Count > 0)
                {
                    var activityMedia = await instaApi.MediaProcessor.GetMediaByIdAsync(userMedia.Value[0].Pk);
                    if (rand.Next(100) > 50 && userMedia.Value.Count > 1)
                        await instaApi.MediaProcessor.GetMediaByIdAsync(userMedia.Value[1].Pk);

                    if (activityMedia.Succeeded)
                    {
                        Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - Success");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - Failed: {activityMedia.Info.Message} - {activityMedia.Info.ResponseType}");
                        return false;
                    }
                }
            }
            return false;
        }
        private async Task<bool> LookupExploreFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion)
        {
            var explore = await instaApi.FeedProcessor.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            if (explore.Succeeded)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Explore - Success");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Explore - Failed: {explore.Info.Message} - {explore.Info.ResponseType}");
                return false;
            }
        }
    }
}