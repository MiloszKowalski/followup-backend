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
using InstagramApiSharp.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            int whileIterations = 0;
            var previousMillieconds = 0;

            // Options for the promotion repository instances
            var options = new DbContextOptionsBuilder<FollowUPContext>()
                .UseSqlServer(_sqlSettings.ConnectionString)
                .Options;

            while (!stoppingToken.IsCancellationRequested)
            {
                whileIterations++;
                Console.WriteLine($"Getting users accounts for the {whileIterations} time...");

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
                var milliseconds = rand.Next(150000, 300000);

                // If the difference between this interval and the previous
                // one is less than 30 seconds, randomize interval again
                while (Math.Abs(previousMillieconds - milliseconds) < 30000)
                    milliseconds = rand.Next(150000, 300000);

                previousMillieconds = milliseconds;

                Parallel.ForEach(accounts, (account) =>
                {
                    var task = Task.Run(async () =>
                    {
                        var _promotionRepository = new PromotionRepository(new FollowUPContext(options, _sqlSettings));
                        // Divide proxy to proper proxy parts
                        var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);
                        var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

                        if (accountProxy == null)
                        {
                            Console.WriteLine($"User {account.Username} doesn't have any working proxy, skipping...");
                            await Task.Delay(5000);
                            return;
                        }
                        else if (proxyInfo.ExpiryDate.ToUniversalTime() < DateTime.UtcNow)
                        {
                            Console.WriteLine($"Proxy {proxyInfo.Ip} for user {account.Username} is expired, skipping...");
                            await Task.Delay(5000);
                            return;
                        }

                        // Try getting EB user's cookies
                        string browserKey = $"{account.Id}-browser";
                        var cacheCookies = (ReadOnlyCollection<OpenQA.Selenium.Cookie>)_cache.Get(browserKey);

                        if (cacheCookies == null)
                        {
                            Console.WriteLine($"User {account.Username} not authenticated to a embedded browser, skipping...");
                            await Task.Delay(5000);
                            return;
                        }

                        await Task.Delay(5000);

                        // Set up proxy used in promotion
                        var proxy = new WebProxy()
                        {
                            Address = new Uri($"http://{proxyInfo.Ip}:{proxyInfo.Port}"),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(
                                userName: proxyInfo.Username,
                                password: proxyInfo.Password)
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

                        instaApi.SetApiVersion(InstaApiVersionType.Version35);
                        instaApi.SetDevice(AndroidDeviceGenerator.GetByName("xiaomi-mi-4w"));

                        // Try logging in from session
                        try
                        {
                            instaApi?.SessionHandler?.Load();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Could not load state from file for user {account.Username}, error info: {e}");
                            await Task.Delay(10000);
                            return;
                        }

                        if(!instaApi.IsUserAuthenticated)
                        {
                            Console.WriteLine($"User {account.Username} not logged in, please authenticate first.");
                            await Task.Delay(1000);
                            return;
                        }


                        string promotionKey = $"{account.Id}-latest-promotion";
                        // Get all account's promotions
                        var promotions = await _promotionRepository.GetAccountPromotionsAsync(account.Id);

                        if (!promotions.Any())
                        {
                            Console.WriteLine($"Couldn't find any promotions for user: {account.Username}, skipping");
                            await Task.Delay(5000);
                            return;
                        }

                        // Queue the promotions to make only one per iteration
                        var currentPromotion = promotions.First();

                        var previousPromotion = (Promotion)_cache.Get(promotionKey);

                        if (previousPromotion != null)
                        {
                            var promotionList = promotions.ToList();
                            var previousPromotionIndex = promotionList.IndexOf(previousPromotion);
                            if (previousPromotionIndex > promotionList.Count - 1)
                                previousPromotionIndex = -1;
                            currentPromotion = promotionList[previousPromotionIndex + 1];
                        }
                        _cache.Set(promotionKey, currentPromotion);

                        // Get all account's comments templates
                        var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(account.Id);
                        var promotion = currentPromotion;
                        
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
                            foreach (var media in medias)
                            {
                                if (completed)
                                    break;
                                var mediaCheck = await _promotionRepository.GetMediaAsync(media.Code, account.Id);
                                if (mediaCheck != null)
                                    if (media.Code == mediaCheck.Code && mediaCheck.AccountId == account.Id)
                                    {
                                        mediasToRemove.Add(media);
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) Skipped media {media?.Caption?.Text?.ToString().Truncate(20)}");
                                        continue;
                                    }

                                ChromeOptions chromeOptions = new ChromeOptions();
                                var chromeProxy = new Proxy
                                {
                                    Kind = ProxyKind.Manual,
                                    IsAutoDetect = false,
                                    SocksUserName = proxyInfo.Username,
                                    SocksPassword = proxyInfo.Password,
                                    HttpProxy = $"http://{proxyInfo.Ip}:{proxyInfo.Port}"
                                };
                                var cookiesPath = Path.Combine(Directory.GetCurrentDirectory(), "ebaccounts", account.Id.ToString());
                                chromeOptions.Proxy = chromeProxy;
                                chromeOptions.AddArgument("user-data-dir=" + cookiesPath);
                                chromeOptions.AddArgument("--lang=en");
                                if (_settings.HeadlessBrowser)
                                    chromeOptions.AddArgument("--headless");
                                chromeOptions.AddArgument("ignore-certificate-errors");
                                using (var chromeDriver = new ChromeDriver(".", chromeOptions))
                                {
                                    try
                                    {
                                        string followUrl = $@"https://www.instagram.com/p/{media.Code}";
                                        chromeDriver.Navigate().GoToUrl(followUrl);

                                        foreach (var cookie in cacheCookies)
                                        {
                                            chromeDriver.Manage().Cookies.AddCookie(cookie);
                                        }
                                        chromeDriver.Navigate().GoToUrl(followUrl);
                                        bool previouslyFollowed = false;
                                        var followButton = chromeDriver.FindElementByCssSelector("button");
                                        if (followButton.Text == "Follow")
                                            followButton.Click();
                                        else if (followButton.Text == "Following")
                                            previouslyFollowed = true;
                                        await Task.Delay(3000);

                                        if (followButton.Text == "Following" && !previouslyFollowed)
                                            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Success!");
                                        else if (previouslyFollowed)
                                            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - User previously followed!");
                                        else
                                        {
                                            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Banned :(");
                                            Console.WriteLine("Turning off promotion...");


                                            // HACK: As the database operations weren't thread safe, it occasionally would crash
                                            // when two threads were saving at the same time. So we repeat this operation
                                            // until it no longer crashes

                                            bool saveSuccessful = false;

                                            while (!saveSuccessful)
                                            {
                                                try
                                                {
                                                    account.PromotionsModuleExpiry = DateTime.UtcNow;
                                                    await _accountRepository.UpdateAsync(account);
                                                    return;
                                                }
                                                catch
                                                {
                                                    saveSuccessful = false;
                                                    Console.WriteLine("Turning off promotion failed. Retrying...");
                                                }
                                                saveSuccessful = true;
                                            }
                                        }

                                        chromeDriver.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                        Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Failed :(");
                                    }
                                }

                                var blackListMedia = new CompletedMedia(Guid.NewGuid(), account.Id, media.Code, DateTime.UtcNow);

                                mediasToRemove.Add(media);

                                Console.WriteLine($"Current media list count: {medias.Count()}");

                                promotion.SetNexMinId(media.InstaIdentifier);
                                await _promotionRepository.UpdateAsync(promotion);
                                await _promotionRepository.AddToBlacklistAsync(blackListMedia);
                                    
                                completed = true;
                                Console.WriteLine($"Waiting {milliseconds} milliseconds");
                                await Task.Delay(milliseconds);
                            }
                                
                            // Remove media from cache
                            foreach (var media in mediasToRemove)
                            {
                                medias.Remove(media);
                            }

                            _cache.Set(searchKey, medias);
                            
                        }
                    });

                    // Wait for each of the tasks to complete
                    task.Wait();
                });

                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Done one full iteration");
            }
        }

        private async Task<List<InstaMedia>> GetMediaByHashtagAsync(IInstaApi instaApi, InstagramAccount account, Promotion promotion, IPromotionRepository promotionRepository)
        {
            int firstHashtagCount = 0;

            var firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(5));
            if (firstHashtagResponse.Info.Message == "challenge_required")
            {
                await instaApi.GetLoggedInChallengeDataInfoAsync();
                await instaApi.AcceptChallengeAsync();
                firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(5));
            }
            
            if (!firstHashtagResponse.Succeeded)
                return new List<InstaMedia>();

            var firstMedia = firstHashtagResponse.Value.Medias;

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
    }
}