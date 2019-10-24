using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Repositories;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.SessionHandlers;
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
        private readonly IMemoryCache _cache;
        private readonly SqlSettings _sqlSettings;

        public Promoter(IInstagramAccountRepository accountRepository,
            IMemoryCache cache, SqlSettings sqlSettings)
        {
            _accountRepository = accountRepository;
            _cache = cache;
            _sqlSettings = sqlSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int whileIterations = 0;
            var previousSeconds = 0;

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
                var rand = new Random();
                var seconds = rand.Next(50, 150);

                while(Math.Abs(previousSeconds - seconds) < 15)
                    seconds = rand.Next(50, 150);

                previousSeconds = seconds;

                Parallel.ForEach(accounts, (account) =>
                {
                    var task = Task.Run(async () =>
                    {
                        var _promotionRepository = new PromotionRepository(new FollowUPContext(options, _sqlSettings));
                        // Divide proxy to proper proxy parts
                        var proxySplit = account.Proxy.Split(':');
                        string proxyIp = proxySplit[0];
                        string proxyPort = proxySplit[1];
                        proxyIp = $"http://{proxyIp}:{proxyPort}";
                        string proxyLogin = proxySplit[2];
                        string proxyPassword = proxySplit[3];

                        // Set up proxy used in promotion
                        var proxy = new WebProxy()
                        {
                            Address = new Uri(proxyIp),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(
                                userName: proxyLogin,
                                password: proxyPassword)
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
                                                    .UseLogger(new DebugLogger(LogLevel.Exceptions))
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

                        var device = instaApi.GetCurrentDevice();

                        if(!instaApi.IsUserAuthenticated)
                        {
                            Console.WriteLine($"User {account.Username} not logged in, please authenticate first.");
                            await Task.Delay(10000);
                            return;
                        }

                        // Get all account's promotions
                        var promotions = await _promotionRepository.GetAccountPromotionsAsync(account.Id);

                        if (!promotions.Any())
                        {
                            return;
                        }

                        // Get all account's comments templates
                        var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(account.Id);

                        foreach (var promotion in promotions)
                        {
                            if (promotion.PromotionType == PromotionType.Hashtag)
                            {
                                var searchKey = $"{account.Id}_{promotion.Label}";
                                List<InstaMedia> medias = (List<InstaMedia>)_cache.Get(searchKey);
                                if(medias == null || !medias.Any())
                                {
                                    medias = _cache.Set(searchKey, await GetMediaByHashtagAsync(instaApi, account, promotion, _promotionRepository));
                                    continue;
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
                                    Console.WriteLine();
                                    var likeResponse = await instaApi.MediaProcessor.LikeMediaAsync(media.InstaIdentifier);
                                    if (likeResponse.Succeeded)
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) -> [{media.User.UserName}] Like media: {media.Caption?.Text?.ToString().Truncate(20)} - Success");
                                    else
                                    {
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) -> [{media.User.UserName}] Like media: {media.Caption?.Text?.ToString().Truncate(20)} - Error: {likeResponse.Info.Message} - {likeResponse.Info.ResponseType}");
                                        Console.WriteLine("Waiting a day...");
                                        await Task.Delay(TimeSpan.FromHours(24));
                                    }

                                    //Console.WriteLine();
                                    //var commentResponse = await instaApi.CommentProcessor.CommentMediaAsync(media.InstaIdentifier, comments.ElementAt(rand.Next(0, comments.Count())).Content);
                                    //if (commentResponse.Succeeded)
                                    //    Console.WriteLine($"[{account.Username}](#{promotion.Label}) -> [{media.User.UserName}] Comment media: {media.Caption?.Text?.ToString().Truncate(20)} - Success!");
                                    //else
                                    //    Console.WriteLine($"[{account.Username}](#{promotion.Label}) -> [{media.User.UserName}] Comment media: {media.Caption?.Text?.ToString().Truncate(20)} - Error: {commentResponse.Info.Message} - {commentResponse.Info.ResponseType}");

                                    //await Task.Delay(TimeSpan.FromSeconds(10));

                                    Console.WriteLine($"Waiting {seconds/1.25} seconds for following");
                                    await Task.Delay(TimeSpan.FromSeconds(seconds/1.25));

                                    Console.WriteLine();
                                    var followResponse = await instaApi.UserProcessor.FollowUserAsync(media.User.Pk);
                                    if (followResponse.Succeeded)
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Success!");
                                    else
                                    {
                                        Console.WriteLine($"[{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Failed: {followResponse.Info.Message} - {followResponse.Info.ResponseType}");
                                        Console.WriteLine("Waiting a day...");
                                        await Task.Delay(TimeSpan.FromHours(24));
                                    }

                                    var blackListMedia = new CompletedMedia(Guid.NewGuid(), account.Id, media.Code, DateTime.UtcNow);

                                    mediasToRemove.Add(media);

                                    Console.WriteLine($"Current media list count: {medias.Count()}");


                                    bool saveSuccessful = false;

                                    // HACK: As the database operations weren't thread safe, it occasionally would crash
                                    // when two threads were saving at the same time. So we repeat this operation
                                    // until it no longer crashes
                                    while (!saveSuccessful)
                                    {
                                        try
                                        {
                                            promotion.SetNexMinId(media.InstaIdentifier);
                                            await _promotionRepository.UpdateAsync(promotion);
                                            await _promotionRepository.AddToBlacklistAsync(blackListMedia);
                                        } catch(Exception ex)
                                        {
                                            saveSuccessful = false;
                                            Console.WriteLine("************************************");
                                            Console.WriteLine(ex);
                                        }
                                        saveSuccessful = true;
                                    }
                                    
                                    completed = true;
                                    Console.WriteLine($"Waiting {seconds} seconds");
                                    await Task.Delay(TimeSpan.FromSeconds(seconds));
                                }
                                
                                foreach (var media in mediasToRemove)
                                {
                                    medias.Remove(media);
                                }

                                _cache.Set(searchKey, medias);
                            }
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