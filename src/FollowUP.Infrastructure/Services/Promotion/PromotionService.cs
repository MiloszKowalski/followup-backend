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
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public async Task SetPromotionCooldown(InstagramAccount account, InstagramAccountRepository accountRepository,
            int minActionInterval = 0, int maxActionInterval = 0)
        { 
            var rand = new Random();
            int milliseconds = 0;
            int previousMilliseconds = 0;

            if(minActionInterval <= 0 || maxActionInterval <= 0 || minActionInterval > maxActionInterval)
            {
                milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
                previousMilliseconds = account.PreviousCooldownMilliseconds;

                // If the difference between this interval and the previous
                // one is less than given number of milliseconds, randomize interval again
                while (Math.Abs(previousMilliseconds - milliseconds) < _settings.MinIntervalDifference)
                    milliseconds = rand.Next(_settings.MinActionInterval, _settings.MaxActionInterval);
                account.SetActionCooldown(milliseconds);
            }
            else
            {
                milliseconds = rand.Next(minActionInterval, maxActionInterval);
                account.SetActionCooldown(milliseconds);
            }

            await accountRepository.UpdateAsync(account);
            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Waiting {milliseconds} milliseconds");
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
                proxy.SetIsTaken(true);
                var previousAccountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);
                var previousProxy = await _proxyRepository.GetAsync(previousAccountProxy.ProxyId);
                previousProxy.SetIsTaken(false);
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
                    account.SetAndroidDevice(AndroidDeviceGenerator.GetRandomName());
                    account.SetBannedUntil(DateTime.UtcNow.AddDays(_settings.BanDurationInDays));
                    account.SetPromotionsModuleExpiry(account.PromotionsModuleExpiry.AddDays(_settings.BanDurationInDays));
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
        public async Task<bool> LookupActivityFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion = null)
        {
            if (promotion == null)
                promotion = new Promotion(Guid.NewGuid(), Guid.NewGuid(), PromotionType.Hashtag, "Unfollow", DateTime.Now);

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
        public async Task<bool> LookupExploreFeed(IInstaApi instaApi, InstagramAccount account, Promotion promotion = null)
        {
            if (promotion == null)
                promotion = new Promotion(Guid.NewGuid(), Guid.NewGuid(), PromotionType.Hashtag, "Unfollow", DateTime.Now);

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
        public async Task<bool> LikeMedia(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, StatisticsService statisticsService, InstaMedia media, int likesDone)
        {
            var likeResponse = await instaApi.MediaProcessor.LikeMediaAsync(media.InstaIdentifier);
            if (likeResponse.Succeeded)
            {
                likesDone++;
                await statisticsService.AddLike(account.Id);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Like media {media.Code} by user: {media.User.UserName} - Success! - number of likes: {likesDone}");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Like media {media.Code} by user: {media.User.UserName} - Failed: {likeResponse.Info.Message} - {likeResponse.Info.ResponseType}");
                if (likeResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                {
                    if(likeResponse.Info.ResponseType == ResponseType.ChallengeRequired)
                    {
                        await instaApi.GetLoggedInChallengeDataInfoAsync();
                        await instaApi.AcceptChallengeAsync(2, "613280");
                        return false;
                    }
                    await ProceedBan(account);
                    return false;
                }
                else
                {
                    var searchKey = $"{account.Id}-{promotion.Label}-hashtag{_settings.SearchKey}";
                    List<InstaMedia> medias = (List<InstaMedia>)_cache.Get(searchKey);

                    // Remove media from cache
                    medias.Remove(media);

                    // Update remaining media list in cache
                    _cache.Set(searchKey, medias);
                }
                return false;
            }
        }
        public async Task<bool> FollowProfile(IInstaApi instaApi, InstagramAccount account, Promotion promotion,
            PromotionRepository promotionRepository, StatisticsService statisticsService, InstagramAccountRepository accountRepository, InstaMedia media, int followsDone)
        {
            var followResponse = await instaApi.UserProcessor.FollowUserAsync(media.User.Pk);
            if (followResponse.Succeeded)
            {
                followsDone++;
                await statisticsService.AddFollow(account.Id);
                string userPk = media.User.Pk.ToString();

                var followedCheck = await promotionRepository.GetFollowedProfileAsync(account.Id, userPk);
                if(followedCheck == null)
                {
                    var followedProfile = new FollowedProfile(Guid.NewGuid(), account.Id, userPk);
                    await promotionRepository.AddFollowedProfileAsync(followedProfile);
                }

                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Success! - number of follows: {followsDone}");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}](#{promotion.Label}) Follow user: {media.User.UserName} - Failed: {followResponse.Info.Message} - {followResponse.Info.ResponseType}");
                if (followResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                {
                    if (followResponse.Info.ResponseType == ResponseType.ChallengeRequired)
                    {
                        await instaApi.GetLoggedInChallengeDataInfoAsync();
                        await instaApi.AcceptChallengeAsync();
                        return false;
                    }
                    await ProceedBan(account);
                    return false;
                }
                return false;
            }
        }
        public async Task<bool> UnfollowProfile(IInstaApi instaApi, InstagramAccount account,
            PromotionRepository promotionRepository, StatisticsService statisticsService, InstagramAccountRepository accountRepository, int unFollowsDone)
        {
            var profileToUnfollow = await promotionRepository.GetRandomFollowedProfileAsync(account.Id);
            if (profileToUnfollow == null)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow - Failed: No profiles to unfollow");
                _cache.Set($"{account.Id}-canUnfollow-{DateTime.Today}", false);
                return false;
            }

            // Dummy request to simulate app following search
            await instaApi.UserProcessor.GetCurrentUserAsync();
            await instaApi.UserProcessor.GetUserFollowingAsync(account.Username, PaginationParameters.MaxPagesToLoad(1));

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
                await statisticsService.AddUnfollow(account.Id);
                await promotionRepository.RemoveFollowedProfileAsync(account.Id, profileToUnfollow.ProfileId);
                await SetPromotionCooldown(account, accountRepository);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow user: {profileName} - Success! - number of unfollows: {unFollowsDone}");
                return true;
            }
            else if (unFollowResponse.Info.ResponseType == ResponseType.UnExpectedResponse)
            {
                await promotionRepository.RemoveFollowedProfileAsync(account.Id, profileToUnfollow.ProfileId);
                await SetPromotionCooldown(account, accountRepository);
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow user: {profileName} - Failed - wrong user - Deleting profile from database.");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Unfollow user: {profileName} - Failed: {unFollowResponse.Info.Message} - {unFollowResponse.Info.ResponseType}");
                if (unFollowResponse.Info.ResponseType != ResponseType.UnExpectedResponse)
                
                {
                    if (unFollowResponse.Info.ResponseType == ResponseType.ChallengeRequired)
                    {
                        await instaApi.GetLoggedInChallengeDataInfoAsync();
                        // TODO: Get proper method and value from account's info
                        await instaApi.AcceptChallengeAsync(1, "+48500067012");
                        return false;
                    }
                    await ProceedBan(account);
                    return false;
                }
            }
            return false;
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

            var promotionList = promotions.OrderBy(x => x.Label).ToList();

            // Queue the promotions to make only one per iteration
            var currentPromotion = promotionList.First();

            var previousPromotion = (Promotion)_cache.Get(promotionKey);

            if (previousPromotion != null)
            {
                int previousPromotionIndex = -1;
                foreach (var promo in promotionList)
                {
                    if (promo.Id == previousPromotion.Id)
                    {
                        previousPromotionIndex = promotionList.IndexOf(promo);
                        break;
                    }
                }
                currentPromotion = promotionList[previousPromotionIndex >= promotionList.Count - 1 ? 0 : (previousPromotionIndex + 1)];
            }
            _cache.Set(promotionKey, currentPromotion);

            return currentPromotion;
        }
    }
}
