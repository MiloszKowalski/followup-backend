using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Services.Logging;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.InstagramApiService
{
    public class InstagramApiService : IInstagramApiService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly IInstaActionLogger _logger;
        private readonly IMemoryCache _cache;

        public InstagramApiService(IProxyRepository proxyRepository, IMemoryCache cache,
                                   IInstagramAccountRepository accountRepository, IInstaActionLogger logger)
        {
            _accountRepository = accountRepository;
            _proxyRepository = proxyRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IInstaApi> GetInstaApi(InstagramAccount account, bool forLogin = false)
        {
            // Divide proxy to proper proxy parts
            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);

            if (accountProxy == null)
            {
                _logger.Log($"Can't find any working proxy", InstaLogLevel.User, account);
                await Task.Delay(5000);
                return null;
            }

            var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

            //if (proxyInfo.ExpiryDate < DateTime.UtcNow)
            //{
            //    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Proxy {proxyInfo.Ip} is expired, skipping...");
            //    await Task.Delay(TimeSpan.FromSeconds(5));
            //    return null;
            //}

            var proxy = new InstaProxy("localhost", "8888");//proxyInfo.Ip, proxyInfo.Port);
            //{
            //    Credentials = new NetworkCredential(proxyInfo.Username, proxyInfo.Password)
            //};

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler() { Proxy = proxy };

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
                                        .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                        .UseHttpClientHandler(httpClientHandler)
                                        .SetDevice(AndroidDeviceGenerator.GetByName(account.AndroidDevice))
                                        .SetApiVersion(InstaApiVersionType.Version130)
                                        .Build();

            // Check if there is an instaApi instance bound to the account...
            var instaApiCache = (IInstaApi)_cache.Get(account.Id);

            if (instaApiCache != null)
            {
                // ...if true, use it
                instaApi = instaApiCache;
            }

            var instaPath = account.FilePath;
            instaPath = instaPath.Replace('\\', Path.DirectorySeparatorChar);
            instaPath = instaPath.Replace('/', Path.DirectorySeparatorChar);

            // Get appropriate directories of the folder and file
            var fullPath = instaPath.Split(Path.DirectorySeparatorChar);
            var directory = Path.Combine(fullPath[0], fullPath[1]);

            // Create directory if it doesn't exist yet
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create file if it doesn't exist yet
            if (!File.Exists(instaApi.SessionHandler.FilePath))
            {
                using (FileStream fs = File.Create(instaApi.SessionHandler.FilePath))
                {
                    _logger.Log($"Created file for user.", InstaLogLevel.Trace, account);
                }
            }

            // Try logging in from session
            try
            {
                instaApi?.SessionHandler?.Load();
            }
            catch (Exception e)
            {
                _logger.Log($" Could not load state from file, error info: {e.Message}", InstaLogLevel.Errors, account);
            }

            if (!instaApi.IsUserAuthenticated && !forLogin)
            {
                _logger.Log($"User not logged in, please authenticate first.", InstaLogLevel.User, account);
                await Task.Delay(TimeSpan.FromSeconds(1));
                return null;
            }

            return instaApi;
        }

        // TODO: Randomize the accounts from the account pool
        public async Task<InstagramAccount> GetRandomSlaveAccount()
        {
            return await _accountRepository.GetAsync("kontotestowefollowup1");
        }

        // TODO: preloaded reel ids
        public async Task SendColdStartMockupRequests(IInstaApi instaApi, InstagramAccount account)
        {
            await instaApi.StoryProcessor.GetStoryFeedWithPostMethodAsync(/* preloaded reel ids */);
            await instaApi.FeedProcessor.GetUserTimelineFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.LauncherSyncAsync(false);
            await instaApi.QeSync(false);
            await instaApi.LauncherSyncAsync(true);
            await instaApi.QeSync(true);
            await instaApi.GetLoomConfigAsync();
            await instaApi.GetLinkageStatusAsync();
            await instaApi.GetBusinessBrandedContentAsync();
            await instaApi.GetBusinessEligibilityAsync();
            await instaApi.GetAccountFamilyAsync();
            await instaApi.FeedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            // arlink
            await instaApi.FeedProcessor.GetTopicalExploreFeedAfterLogin();
            await instaApi.PushProcessor.RegisterPushAsync(InstaPushChannelType.Mqtt);
            await instaApi.BusinessProcessor.GetBusinessAccountInformationAsync();
            await instaApi.MessagingProcessor.GetUsersPresenceAsync();
            await instaApi.GetViewableStatusesAsync();
            await instaApi.GetBanyanSuggestionsAsync();
            await instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1), 1);
            await instaApi.GetUserScoresAsync();
            await instaApi.GetNotificationBadge();
            instaApi.SetDevice(instaApi.GetCurrentDevice().RandomizeBandwithConnection());
            _logger.Log("Sent cold startup mockup requests.", InstaLogLevel.Info, account);
        }

        public async Task GetUserProfileMockAsync(IInstaApi instaApi, InstagramAccount account)
        {
            await instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.IstagramOtherLoggedInUserIdLoaded, true);
            var userPk = instaApi.GetLoggedUser().LoggedInUser.Pk;
            await instaApi.GetInitialUserFeedAsync(userPk);
            await instaApi.UserProcessor.GetUserInfoByIdAsync(userPk, true);
            await instaApi.GetUserFeedCapabilities(userPk);
            await instaApi.StoryProcessor.GetHighlightFeedsAsync(userPk);
            await instaApi.GetProfileSuBadge();
            await instaApi.GetProfileArchiveBadge();
            _logger.Log("Sent user profile mock requests.", InstaLogLevel.Info, account);
        }

        public async Task GetUserFollowedAsync(IInstaApi instaApi, InstagramAccount account)
        {
            var userPk = instaApi.GetLoggedUser().LoggedInUser.Pk;
            var followingResponse = await instaApi.UserProcessor.GetUserFollowingByIdAsync(userPk, PaginationParameters.MaxPagesToLoad(1));
            await instaApi.UserProcessor.GetUserFollowersByIdAsync(userPk, PaginationParameters.MaxPagesToLoad(1));
            await instaApi.UserProcessor.GetSuggestionUsersAsync(PaginationParameters.MaxPagesToLoad(1), "self_followers");
            await instaApi.UserProcessor.GetSuggestionUsersAsync(PaginationParameters.MaxPagesToLoad(1), "self_following");
            var following = followingResponse.Value;
            var followingIds = new List<long>();

            foreach (var followed in following)
                followingIds.Add(followed.Pk);

            var followedResponse = await instaApi.UserProcessor.GetFriendshipStatusesAsync(followingIds.ToArray());

            if (followedResponse.Succeeded)
            {
                _cache.Set($"{account.Id}-followed", followedResponse.Value);
                _logger.Log($"Got followed list succesfully.", InstaLogLevel.User, account);
            }
            else
            {
                _logger.Log($"Getting followed list failed.", InstaLogLevel.User, account);
            }
        }

        public async Task UnfollowUsersAsync(IInstaApi instaApi, InstagramAccount account, int count)
        {
            var random = new Random();
            var profiles = (InstaFriendshipShortStatusList)_cache.Get($"{account.Id}-followed");
            var unfollowedProfiles = new InstaFriendshipShortStatusList();

            if (profiles.Count < count)
                count = profiles.Count;

            for (int i = 0; i < count; i++)
            {
                var unfollowResponse = await instaApi.UserProcessor.UnFollowUserAsync(profiles[i].Pk);

                if (unfollowResponse.Succeeded)
                {
                    unfollowedProfiles.Add(profiles[i]);
                    _logger.Log($"Unfollowed profile {profiles[i].Pk} succesfully", InstaLogLevel.User, account, null);
                }
                else
                {
                    _logger.Log($"Failed to unfollow profile {profiles[i].Pk} - {unfollowResponse.Info.ToString()}", InstaLogLevel.User, account, null);
                    return;
                }

                if (i % 6 == 0)
                    await Task.Delay(random.Next(2000, 5000));
                else
                    await Task.Delay(random.Next(800, 3000));
            }

            profiles.RemoveRange(0, count);
            _cache.Set($"{account.Id}-followed", profiles);
        }

        public async Task GetHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag)
        {
            instaApi.SetDevice(instaApi.GetCurrentDevice().RandomizeBandwithConnection());
            await instaApi.DiscoverProcessor.GetSuggestedSearchesAsync(InstaDiscoverSearchType.Users);
            await instaApi.DiscoverProcessor.GetNullStateDynamicSections();
            await instaApi.DiscoverProcessor.GetRecentSearchesAsync();
            await instaApi.FeedProcessor.GetTopicalExploreFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.InstagramExploreHeader);

            string[] tagParts = tag.Split(null, 3);
            string tagQuery = "";

            for (int i = 0; i < tagParts.Length - 1; i++)
            {
                if (i < tagParts.Length - 2) tagQuery += tagParts[i + 1];
                await instaApi.HashtagProcessor.SearchHashtagAsync(tagQuery);
            }

            var hashtagRepsonse = await instaApi.HashtagProcessor.SearchHashtagAsync(tag);

            long entityId = 0;
            if (hashtagRepsonse.Succeeded)
                entityId = hashtagRepsonse.Value.FirstOrDefault(x => x.Name == tag).Id;

            if (entityId == 0)
                return;

            await instaApi.DiscoverProcessor.RegisterRecentSearchClickAsync(entityId, InstaEntityType.Hashtag);
            await instaApi.HashtagProcessor.GetHashtagsSectionsAsync(tag, PaginationParameters.MaxPagesToLoad(1));
            await instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.InstagramHashtagFeedTooltip);
            var stories = await instaApi.HashtagProcessor.GetHashtagStoriesAsync(tag);
            await instaApi.HashtagProcessor.GetHashtagInfoAsync(tag);

            var media = await instaApi.HashtagProcessor.GetHashtagsSectionsAsync(tag, PaginationParameters.MaxPagesToLoad(1), true, InstaHashtagSectionType.Recent);
            if (!media.Succeeded)
                return;

            _cache.Set($"{account.Id}-hashtag-{tag}-media", media.Value.Medias);

            _logger.Log($"Fetched media from tag {tag} successfully", InstaLogLevel.User, account);
        }

        public async Task LikeHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag, int count)
        {
            var random = new Random();
            var media = (List<InstaMedia>)_cache.Get($"{account.Id}-hashtag-{tag}-media");

            if (media.Count < count)
                count = media.Count;

            for (int i = 0; i < count; i++)
            {
                var likeResponse = await instaApi.MediaProcessor.LikeMediaAsync(media[i].InstaIdentifier, i + 1);

                if (likeResponse.Succeeded)
                {
                    _logger.Log($"Liked media {media[i].Code} succesfully", InstaLogLevel.User, account, null);
                    await Task.Delay(random.Next(750, 3000));
                    var followResponse = await instaApi.UserProcessor.FollowUserAsync(media[i].User.Pk);
                    if(followResponse.Succeeded)
                    {
                        _logger.Log($"Followed user {media[i].User.Pk} succesfully", InstaLogLevel.User, account, null);
                        var suggestionsResponse = await instaApi.DiscoverProcessor.GetAccountsRecsAsync(instaApi.GetLoggedUser().LoggedInUser.Pk);
                        if(suggestionsResponse.Succeeded)
                            _logger.Log($"Got suggestions after following user {media[i].User.Pk} succesfully", InstaLogLevel.Info, account, null);
                        else
                            _logger.Log($"Getting suggestions after following user {media[i].User.Pk} failed - {suggestionsResponse.Info.ToString()}", InstaLogLevel.Info, account, null);
                    }
                    else
                    {
                        _logger.Log($"Failed to follow user {media[i].User.Pk} - {followResponse.Info.ToString()}", InstaLogLevel.User, account, null);
                        return;
                    }
                }
                else
                {
                    _logger.Log($"Failed to like media {media[i].Code} - {likeResponse.Info.ToString()}", InstaLogLevel.User, account, null);
                    return;
                }

                await Task.Delay(random.Next(2000, 5000));
            }

            media.RemoveRange(0, count);
            _cache.Set($"{account.Id}-hashtag-{tag}-media", media);
        }
    }
}
