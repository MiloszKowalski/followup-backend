using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Services.Logging;
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

namespace FollowUP.Infrastructure.Services.InstagramApiService
{
    public class InstagramApiService : IInstagramApiService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly IInstaActionLogger _logger;
        private readonly PromotionSettings _settings;
        private readonly IAesEncryptor _encryptor;
        private readonly IMemoryCache _cache;

        public InstagramApiService(IProxyRepository proxyRepository, IMemoryCache cache,
                                   IInstagramAccountRepository accountRepository, IInstaActionLogger logger,
                                   PromotionSettings settings, IAesEncryptor encryptor)
        {
            _accountRepository = accountRepository;
            _proxyRepository = proxyRepository;
            _logger = logger;
            _settings = settings;
            _encryptor = encryptor;
            _cache = cache;
        }

        public async Task<IInstaApi> GetInstaApiAsync(InstagramAccount account, bool forLogin = false)
        {
            InstaProxy proxy = null;
            if(_settings.UseProxy)
            {
                // Divide proxy to proper proxy parts
                var accountProxy = account.InstagramProxy;

                if (accountProxy == null)
                {
                    _logger.LogUser($"Can't find any working proxy", account);
                    await Task.Delay(5000);
                    return null;
                }

                if (accountProxy.ExpiryDate < DateTime.UtcNow)
                {
                    Console.WriteLine($"[{DateTime.Now}][{account.Username}] Proxy {accountProxy.Ip} is expired, skipping...");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return null;
                }

                proxy = new InstaProxy(accountProxy.Ip, accountProxy.Port)
                {
                    Credentials = new NetworkCredential(accountProxy.Username, accountProxy.Password)
                };
            }
            else if (_settings.UseLocalProxy)
            {
                proxy = new InstaProxy("127.0.0.1", "8888");
            }
            
            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler() { Proxy = proxy };

            // Set the user's credentials
            var userSession = new UserSessionData
            {
                UserName = account.Username,
                Password = _encryptor.Decrypt(account.Password)
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
            var instaApiCache = (IInstaApi)_cache.Get(account.Username);

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
            {
                Directory.CreateDirectory(directory);
            }

            // Create file if it doesn't exist yet
            if (!File.Exists(instaApi.SessionHandler.FilePath))
            {
                using (FileStream fs = File.Create(instaApi.SessionHandler.FilePath))
                {
                    _logger.LogTrace($"Created file for user.", account);
                }
            }

            // Try logging in from session
            try
            {
                instaApi?.SessionHandler?.Load();
            }
            catch (Exception e)
            {
                _logger.LogError($" Could not load state from file, error info: {e.Message}", account);
            }

            if (!instaApi.IsUserAuthenticated && !forLogin)
            {
                _logger.LogUser($"User not logged in, please authenticate first.", account);
                await Task.Delay(TimeSpan.FromSeconds(1));
                return null;
            }

            return instaApi;
        }

        // TODO: Randomize the accounts from the account pool
        public async Task<InstagramAccount> GetRandomSlaveAccountAsync()
        {
            return await _accountRepository.GetAsync("kontotestowefollowup1");
        }

        // TODO: preloaded reel ids
        public async Task SendColdStartMockupRequestsAsync(IInstaApi instaApi, InstagramAccount account)
        {
            var coldStartRequests = new List<Task>
            {
                instaApi.StoryProcessor.GetStoryFeedWithPostMethodAsync(/* preloaded reel ids */),
                instaApi.FeedProcessor.GetUserTimelineFeedAsync(PaginationParameters.MaxPagesToLoad(1)),
                instaApi.LauncherSyncAsync(false),
                instaApi.QeSync(false),
                instaApi.LauncherSyncAsync(true),
                instaApi.QeSync(true),
                instaApi.DiscoverProcessor.MarkSuSeen(),
                instaApi.MediaProcessor.GetBlockedMediasAsync(),
                instaApi.StoryProcessor.GetUsersStoriesAsHighlightsAsync(instaApi.GetLoggedUser().LoggedInUser.Pk.ToString()),
                instaApi.GetLoomConfigAsync(),
                instaApi.GetLinkageStatusAsync(),
                // Not required(?): instaApi.GetBusinessBrandedContentAsync()),
                instaApi.GetBusinessEligibilityAsync(),
                instaApi.GetAccountFamilyAsync(),
                instaApi.GetArlinkDownloadInfo(),
                instaApi.FeedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1), false),
                instaApi.GetViewableStatusesAsync(),
                instaApi.FeedProcessor.GetTopicalExploreFeedAfterLogin(),
                instaApi.BusinessProcessor.GetBusinessAccountInformationAsync(),
                instaApi.GetBanyanSuggestionsAsync(),
                instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1), 1),
                instaApi.GetNotificationBadge(),
                instaApi.PushProcessor.RegisterPushAsync(InstaPushChannelType.Mqtt),
                instaApi.MessagingProcessor.GetUsersPresenceAsync(),
                instaApi.GetUserScoresAsync(),
            };

            await Task.WhenAll(coldStartRequests);

            instaApi.SetDevice(instaApi.GetCurrentDevice().RandomizeBandwithConnection());
            _logger.LogInfo("Sent cold startup mockup requests.", account);
        }

        public async Task GetUserProfileMockAsync(IInstaApi instaApi, InstagramAccount account)
        {
            var userPk = instaApi.GetLoggedUser().LoggedInUser.Pk;
            var userProfileRequests = new List<Task>()
            {
                instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.IstagramOtherLoggedInUserIdLoaded, true),
                instaApi.GetInitialUserFeedAsync(userPk),
                instaApi.UserProcessor.GetUserInfoByIdAsync(userPk, true),
                instaApi.GetUserFeedCapabilities(userPk),
                instaApi.StoryProcessor.GetHighlightFeedsAsync(userPk),
                instaApi.GetProfileSuBadge(),
                instaApi.GetProfileArchiveBadge(),
            };
            
            await Task.WhenAll(userProfileRequests);

            _logger.LogInfo("Sent user profile mock requests.", account);
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
                _logger.LogUser($"Got followed list succesfully.", account);
            }
            else
            {
                _logger.LogUser($"Getting followed list failed.", account);
            }
        }

        public async Task UnfollowUserAsync(IInstaApi instaApi, InstagramAccount account)
        {
            var profiles = (InstaFriendshipShortStatusList)_cache.Get($"{account.Id}-followed");

            if(profiles == null || profiles.Count <= 0)
            {
                await GetUserFollowedAsync(instaApi, account);
                profiles = (InstaFriendshipShortStatusList)_cache.Get($"{account.Id}-followed");
            }

            var profile = profiles.First();

            var unfollowResponse = await instaApi.UserProcessor.UnFollowUserAsync(profile.Pk);

            if (unfollowResponse.Succeeded)
            {
                _logger.LogUser($"Unfollowed profile {profile.Pk} succesfully", account, null);
            }
            else
            {
                _logger.LogUser($"Failed to unfollow profile {profile.Pk} - {unfollowResponse.Info}", account, null);
                return;
            }

            profiles.Remove(profile);
            _cache.Set($"{account.Id}-followed", profiles);
        }

        public async Task GetHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag)
        {
            instaApi.SetDevice(instaApi.GetCurrentDevice().RandomizeBandwithConnection());
            await instaApi.DiscoverProcessor.GetSuggestedSearchesAsync(InstaDiscoverSearchType.Users);
            await instaApi.DiscoverProcessor.GetDynamicSearchesAsync(InstaDiscoverSearchType.Blended);
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

            var media = await instaApi.HashtagProcessor.GetHashtagsSectionsAsync(tag, PaginationParameters.MaxPagesToLoad(1),
                                                                                        true, InstaHashtagSectionType.Recent);
            if (!media.Succeeded)
                return;

            _cache.Set($"{account.Id}-hashtag-{tag}-media", media.Value.Medias);
            _cache.Set($"{account.Id}-hashtag-{tag}-media-nextId", media.Value.Medias.Last().Pk);

            _logger.LogUser($"Fetched media from tag {tag} successfully", account);
        }

        public async Task LikeHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag, int count)
        {
            var random = new Random();
            var media = (List<InstaMedia>)_cache.Get($"{account.Id}-hashtag-{tag}-media");

            if(media == null || media.Count <= 0)
            {
                await GetHashtagMediaAsync(instaApi, account, tag);
            }

            if (media.Count < count)
                count = media.Count;

            // instaApi.UserProcessor.MuteUserMediaAsync() for muting
            var likeResponse = await instaApi.MediaProcessor.LikeMediaAsync(media[count].InstaIdentifier, count + 1);

            if (likeResponse.Succeeded)
            {
                _logger.LogUser($"Liked media {media[count].Code} succesfully", account, null);
                await Task.Delay(random.Next(750, 3000));
                await FollowUserAsync(instaApi, account, media[count].User.Pk, media[count].Pk, false);
            }
            else
            {
                _logger.LogUser($"Failed to like media {media[count].Code} - {likeResponse.Info}", account, null);
                return;
            }

            media.Remove(media[count]);
            _cache.Set($"{account.Id}-hashtag-{tag}-media", media);
        }

        private async Task<bool> FollowUserAsync(IInstaApi instaApi, InstagramAccount account, long userId,
                                                                string mediaId = null, bool muteUser = false)
        {
            if(muteUser) await GetUserProfileAsync(instaApi, userId);
            var followResponse = await instaApi.UserProcessor.FollowUserAsync(userId, mediaId);
            if (followResponse.Succeeded)
            {
                _logger.LogUser($"Followed user {userId} succesfully", account, null);

                if(muteUser)
                {
                    var random = new Random();
                    await instaApi.DiscoverProcessor.GetChainingUsersAsync(userId);
                    await instaApi.GetUnfollowChainingCount(userId);
                    await instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.InstagramSurvey, true);
                    await Task.Delay(random.Next(300, 500));
                    await instaApi.UserProcessor.MuteUserMediaAsync(userId, InstaMuteOption.Post);
                    await Task.Delay(random.Next(300, 500));
                    await instaApi.UserProcessor.MuteUserMediaAsync(userId, InstaMuteOption.Story);
                }
                else
                {
                    await instaApi.DiscoverProcessor.GetAccountsRecsAsync(userId);
                }
            }
            else
            {
                _logger.LogUser($"Failed to follow user {userId} - {followResponse.Info}", account, null);
            }

            return followResponse.Succeeded;
        }

        private async Task GetUserProfileAsync(IInstaApi instaApi, long userId)
        {
            var tasks = new List<Task> {
                instaApi.UserProcessor.GetFriendshipStatusAsync(userId),
                instaApi.UserProcessor.GetUserMediaByIdAsync(userId, PaginationParameters.MaxPagesToLoad(2)),
                instaApi.UserProcessor.GetUserInfoByIdAsync(userId, false),
                instaApi.QpBatchFetch(InstaQpBatchFetchSurfaceType.GetUserProfile, true),
                instaApi.GetUserFeedCapabilities(userId),
                instaApi.StoryProcessor.GetHighlightFeedsAsync(userId),
                instaApi.TVProcessor.GetChannelByIdAsync(userId, PaginationParameters.MaxPagesToLoad(1))
            };

            await Task.WhenAll(tasks);
        }
    }
}
