using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
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
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IInstagramAccountService _accountService;
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly PromotionSettings _settings;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        public PromotionService(IPromotionRepository promotionRepository, IInstagramAccountRepository accountRepository,
            IInstagramAccountService accountService, IProxyRepository proxyRepository, PromotionSettings settings,
            IMemoryCache cache, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _accountRepository = accountRepository;
            _accountService = accountService;
            _proxyRepository = proxyRepository;
            _settings = settings;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsByAccountId(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            var promotions = await _promotionRepository.GetAccountPromotionsAsync(accountId);

            if (promotions == null)
                throw new ServiceException(ErrorCodes.NoPromotions, $"Promotions list for the account '{accountId}' is empty.");

            var promotionDtos = _mapper.Map<IEnumerable<PromotionDto>>(promotions);

            return promotionDtos;
        }

        public async Task<IEnumerable<PromotionCommentDto>> GetAllPromotionCommentsByAccountId(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            var comments = await _promotionRepository.GetAccountsPromotionCommentsAsync(accountId);

            if (comments == null)
                throw new ServiceException(ErrorCodes.NoComments, $"Comment list for the account '{accountId}' is empty.");

            var commentDtos = _mapper.Map<IEnumerable<PromotionCommentDto>>(comments);

            return commentDtos;
        }

        public async Task CreatePromotion(Guid accountId, PromotionType promotionType, string label)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (label.Empty())
                throw new ServiceException(ErrorCodes.LabelIsEmpty, "Label content cannot be empty.");

            if (label.Length > 100)
                throw new ServiceException(ErrorCodes.LabelTooLong, "Promotion label (hashtag or username) must be at most 100 characters long.");

            if (promotionType != PromotionType.Hashtag && promotionType != PromotionType.InstagramProfile)
                throw new ServiceException(ErrorCodes.InvalidPromotionType, "Promotion type given by user isn't supported.");

            var promotion = new Promotion(Guid.NewGuid(), accountId, promotionType, label, DateTime.UtcNow);

            await _promotionRepository.AddAsync(promotion);
        }

        public async Task CreatePromotionComment(Guid accountId, string content)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (content.Empty())
                throw new ServiceException(ErrorCodes.CommentIsEmpty, "Comment content cannot be empty.");

            if (content.Length > 100)
                throw new ServiceException(ErrorCodes.CommentTooLong, "Comment must be at most 100 characters long.");

            var comment = new PromotionComment(Guid.NewGuid(), account.Id, content, DateTime.UtcNow);

            await _promotionRepository.AddPromotionCommentAsync(comment);
        }
        public async Task SetPromotionCooldown(InstagramAccount account, PromotionRepository promotionRepository, Promotion promotion)
        {
            var rand = new Random();
            var milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
            var previousMilliseconds = promotion.PreviousCooldownMilliseconds;

            // If the difference between this interval and the previous
            // one is less than given number of milliseconds, randomize interval again
            while (Math.Abs(previousMilliseconds - milliseconds) < _settings.MinIntervalDifference)
                milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
            promotion.SetActionCooldown(milliseconds);

            await promotionRepository.UpdateAsync(promotion);
            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Waiting {milliseconds} milliseconds");
        }
        public async Task ReLoginUser(InstagramAccount account)
        {
            var filePath = account.FilePath;
            filePath = filePath.Replace('\\', Path.DirectorySeparatorChar);
            filePath = filePath.Replace('/', Path.DirectorySeparatorChar);

            // Delete directory if it exists
            if (File.Exists(filePath))
                File.Delete(filePath);

            InstagramProxy availableProxy = null;

            var allProxies = await _proxyRepository.GetAllAsync();

            foreach (var proxy in allProxies)
            {
                if (proxy.ExpiryDate.ToUniversalTime() < DateTime.UtcNow || proxy.IsTaken)
                    continue;

                availableProxy = proxy;
                proxy.IsTaken = true;
                var previousAccountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);
                var previousProxy = await _proxyRepository.GetAsync(previousAccountProxy.ProxyId);
                previousProxy.IsTaken = false;
                await _proxyRepository.UpdateAsync(previousProxy);
                await _proxyRepository.UpdateAsync(proxy);
                break;
            }

            if (availableProxy == null)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] No proxy available to change.");
                // TODO: Notify about the lack of proxies
                // TODO: Notify the client about login requirement
                return;
            }

            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);
            accountProxy.ProxyId = availableProxy.Id;
            await _proxyRepository.UpdateAccountsProxyAsync(accountProxy);

            await _accountService.LoginAsync(account.Username, account.Password, account.PhoneNumber, "", "", true, false);

            var accountAfterLogin = await _accountRepository.GetAsync(account.Username);

            if (accountAfterLogin.AuthenticationStep != AuthenticationStep.Authenticated)
                //TODO: Notify the client about verification requirement
                Console.WriteLine("Verification required.");
        }
        public async Task ProceedBan(InstagramAccount account)
        {
            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Turning off promotion...");

            await ReLoginUser(account);

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
        public async Task<bool> LookupActivityFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion)
        {
            Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Activity - Start");

            var rand = new Random();
            string activityKey = $"{account.Id}{_settings.ActivityKey}";
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
        public async Task<bool> LookupExploreFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion)
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
        public async Task LikeMedia(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, InstaMedia media, int likesDone)
        {
            var likeResponse = await instaApi.MediaProcessor.LikeMediaAsync(media.InstaIdentifier);
            if (likeResponse.Succeeded)
            {
                likesDone++;
                _cache.Set($"{account.Id}-likes-count", likesDone);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Like media {media.Code} by user: {media.User.UserName} - Success! - number of likes: {likesDone}");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Like media {media.Code} by user: {media.User.UserName} - Failed: {likeResponse.Info.Message} - {likeResponse.Info.ResponseType}");
                if (likeResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                {
                    await ProceedBan(account);
                }
            }
        }
        public async Task FollowProfile(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, InstaMedia media, int followsDone)
        {
            var followResponse = await instaApi.UserProcessor.FollowUserAsync(media.User.Pk);
            if (followResponse.Succeeded)
            {
                followsDone++;
                _cache.Set($"{account.Id}-follows-count", followsDone);
                string userPk = media.User.Pk.ToString();
                var followedProfile = new FollowedProfile(Guid.NewGuid(), account.Id, userPk);
                await _promotionRepository.AddFollowedProfileAsync(followedProfile);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Success! - number of follows: {followsDone}");
                await SetPromotionCooldown(account, promotionRepository, promotion);
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Failed: {followResponse.Info.Message} - {followResponse.Info.ResponseType}");
                if (followResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                {
                    await ProceedBan(account);
                }
            }
        }
        public async Task<bool> UnfollowProfile(IInstaApi instaApi, InstagramAccount account, Promotion promotion, 
            PromotionRepository promotionRepository, int unFollowsDone)
        {
            // Dummy request to simulate app following search
            await instaApi.UserProcessor.GetUserFollowingAsync(account.Username, PaginationParameters.MaxPagesToLoad(1));
            var profileToUnfollow = await promotionRepository.GetRandomFollowedProfileAsync(account.Id);
            if (profileToUnfollow == null)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow - Failed: No profiles to unfollow");
                return false;
            }

            long profileId;

            try
            {
                long.TryParse(profileToUnfollow.ProfileId, out profileId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow - Failed: Error while parsing profile's id - {ex.Message}");
                return false;
            }

            var user = await instaApi.UserProcessor.GetUserInfoByIdAsync(profileId);

            string profileName = "";

            if (user.Succeeded)
                profileName = user.Value.Username;

            var unFollowResponse = await instaApi.UserProcessor.UnFollowUserAsync(profileId);
            if (unFollowResponse.Succeeded)
            {
                unFollowsDone++;
                _cache.Set($"{account.Id}-unfollows-count", unFollowsDone);
                await SetPromotionCooldown(account, promotionRepository, promotion);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow user: {profileName} - Success! - number of unfollows: {unFollowsDone}");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow user: {profileName} - Failed: {unFollowResponse.Info.Message} - {unFollowResponse.Info.ResponseType}");
                if (unFollowResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                {
                    await ProceedBan(account);
                    return false;
                }
            }
            return false;
        }
        public async Task<IInstaApi> GetInstaApi(InstagramAccount account)
        {
            // Divide proxy to proper proxy parts
            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);

            if (accountProxy == null)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Can't find any working proxy, skipping...");
                await Task.Delay(5000);
                return null;
            }

            var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

            if (proxyInfo.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Proxy {proxyInfo.Ip} is expired, skipping...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                return null;
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
                await Task.Delay(TimeSpan.FromSeconds(10));
                return null;
            }

            if (!instaApi.IsUserAuthenticated)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] User not logged in, please authenticate first.");
                await Task.Delay(TimeSpan.FromSeconds(1));
                return null;
            }

            return instaApi;
        }
        public async Task<Promotion> GetCurrentPromotion(InstagramAccount account)
        {
            // Get all account's promotions
            string promotionKey = $"{account.Id}{_settings.PromotionKey}";
            var promotions = await _promotionRepository.GetAccountPromotionsAsync(account.Id);

            if (promotions == null || !promotions.Any())
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Couldn't find any promotions, skipping");
                await Task.Delay(TimeSpan.FromSeconds(5));
                return null;
            }

            // Queue the promotions to make only one per iteration
            var currentPromotion = promotions.First();

            var previousPromotion = (Promotion)_cache.Get(promotionKey);

            if (previousPromotion != null)
            {
                var promotionList = promotions.ToList();

                int previousPromotionIndex = -1;
                foreach (var promo in promotionList)
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
                    return null;
                }
                currentPromotion = promotionList[previousPromotionIndex >= promotionList.Count - 1 ? 0 : (previousPromotionIndex + 1)];
                var timeDifference = (int)previousPromotion.ActionCooldown.Subtract(DateTime.UtcNow).TotalMilliseconds;
                if (timeDifference > 0)
                    currentPromotion.SetActionCooldown(timeDifference);
            }
            _cache.Set(promotionKey, currentPromotion);

            return currentPromotion;
        }
    }
}
