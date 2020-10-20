using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Services.Logging;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp.API;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class PromotionBot
    {
        private readonly IMemoryCache _cache;
        private readonly PromotionSettings _settings;
        private readonly IInstagramApiService _apiService;
        private readonly IInstaActionLogger _logger;
        private readonly IScheduleService _scheduleService;

        private Random Random { get; set; } = new Random();
        public Queue<PromotionTask> Queue { get; private set; }
        private InstagramAccount Profile { get; set; }
        private IInstaApi InstaApi { get; set; }
        public AccountSettings AccountSettings { get; set; }
        public IPromotion PreviousPromotion { get; private set; }
        private int ActionCount { get; set; }
        private int BatchActionCount { get; set; }

        public PromotionBot(IMemoryCache cache, PromotionSettings settings,
            IInstagramApiService apiService, IInstaActionLogger logger, IInstaApi instaApi,
            InstagramAccount profile, IScheduleService scheduleService)
        {
            _cache = cache;
            _settings = settings;
            _apiService = apiService;
            _logger = logger;
            _scheduleService = scheduleService;
            InstaApi = instaApi;
            Profile = profile;
            Queue = new Queue<PromotionTask>();
        }

        public async void Promote()
        {
            // Try to schedule queue at the start
            await TryToScheduleQueueForTodayAsync();

            // Wait for the schedule
            while (Queue.Count <= 0)
            {
                GetPromotionSchedule();
                Console.WriteLine($"Waiting for promotion for account {Profile.Username}.");
                await Task.Delay(10000);
            }

            // For testing purposes only
            int counter = 1;
            while(Queue.Count > 0)
            {
                var promotion = Queue.Dequeue().Promotion;
                if (promotion is FollowPromotion f)
                {
                    Console.WriteLine($"[{counter}] Queue: {f.Label}");
                }
                else
                {
                    Console.WriteLine($"[{counter}] Queue: Unfollow");
                }
                counter++;
            }

            // Send cold start requests for Instagram
            await _apiService.SendColdStartMockupRequestsAsync(InstaApi, Profile);
            
            // As long as account has promotion module bought
            while(Profile.PromotionsModuleExpiry > DateTime.UtcNow)
            {
                // Wait for profile unban
                await CheckForProfileBannedAsync();

                // Night break for it to work 24/7
                // Additionally, it will fetch the new day's
                // promotion queue
                await CheckForNightBreakAsync();

                // Update the settings if they have changed
                if (HaveSettingsChanged())
                {
                    GetUpdatedSettings();
                }

                // Check if the action count exceeded limits
                // If so, wait up to the next day
                await CheckForActionLimitExceededAsync();

                // Update the schedule if it has changed
                if(HasScheduleChanged())
                {
                    GetPromotionSchedule();
                }

                // Try to dequeue the next task, otherwise skip
                if (!Queue.TryDequeue(out var promotionTask))
                {
                    continue;
                }

                var promotion = promotionTask.Promotion;

                if (promotion is UnfollowPromotion)
                {
                    await DoUnfollowAsync();
                }
                else if (promotion is HashtagPromotion h)
                {
                    await DoHashtagAsync(h.Label);
                }
                else if (promotion is ProfilePromotion p)
                {
                    await DoProfileAsync(p.Label);
                }
                else if (promotion is LocationPromotion l)
                {

                }
                else
                {
                    _logger.LogError("An error occured while getting promotion type.", Profile);
                }

                PreviousPromotion = promotion;
                ActionCount++;

                // Wait for a random amount of time
                await Task.Delay(Random.Next(AccountSettings.MinIntervalMilliseconds,
                    AccountSettings.MaxIntervalMilliseconds));

                // Get user settings for promotion
                // Check if the promotion module is paused
            }
        }

        private bool HasScheduleChanged()
        {
            bool scheduleChanged;
            try
            {
                scheduleChanged = (bool)_cache.Get($"{Profile.Id}-schedule-changed");
            }
            catch
            {
                scheduleChanged = false;
            }

            return scheduleChanged;
        }

        private void GetPromotionSchedule()
        {
            var queue = (Queue<PromotionTask>)_cache.Get($"{Profile.Id}-schedule");
            if (queue == null)
            {
                return;
            }

            _cache.Set($"{Profile.Id}-schedule-changed", false);

            Queue = queue;
        }

        private bool HaveSettingsChanged()
        {
            bool settingsChanged;
            try
            {
                settingsChanged = (bool)_cache.Get($"{Profile.Id}-settings-changed");
            }
            catch
            {
                settingsChanged = false;
            }

            return settingsChanged;
        }

        private void GetUpdatedSettings()
        {
            // TODO: Implement getting updated settings
            throw new NotImplementedException();
        }

        private async Task DoUnfollowAsync()
        {
            if(!(PreviousPromotion is UnfollowPromotion))
            {
                await _apiService.GetUserProfileMockAsync(InstaApi, Profile);
                BatchActionCount = 0;
            }

            await _apiService.UnfollowUserAsync(InstaApi, Profile);
            BatchActionCount++;
        }

        private async Task DoHashtagAsync(string tag)
        {
            if (!(PreviousPromotion is HashtagPromotion))
            {
                await _apiService.GetHashtagMediaAsync(InstaApi, Profile, tag);
                BatchActionCount = 0;
            }

            await _apiService.LikeHashtagMediaAsync(InstaApi, Profile, tag, BatchActionCount);
            BatchActionCount++;
        }

        private async Task DoProfileAsync(string profile)
        {
            if (!(PreviousPromotion is ProfilePromotion))
            {
                await _apiService.GetUserProfileMockAsync(InstaApi, Profile);
                BatchActionCount = 0;
            }

            BatchActionCount++;
        }

        private async Task CheckForProfileBannedAsync()
        {
            if (Profile.BannedUntil == null)
            {
                return;
            }
            else if (Profile.BannedUntil > DateTime.UtcNow)
            {
                var interval = (Profile.BannedUntil - DateTime.UtcNow).Milliseconds;
                _logger.LogUser($"Account banned until {Profile.BannedUntil}, " +
                    $"waiting for {interval} milliseconds...", Profile);
                await Task.Delay(interval);
            }
        }

        private async Task CheckForNightBreakAsync()
        {
            if (DateTime.Now < DateTime.Today.AddHours(_settings.StartingHour)
                || DateTime.Now > DateTime.Today.AddHours(_settings.EndingHour))
            {
                _logger.LogUser($"Night break. - ({_settings.EndingHour}:00 - {_settings.StartingHour}:00) " +
                    $"- Will resume at {_settings.StartingHour} o' clock!", Profile);
                await Task.Delay(TimeSpan.FromHours(24 + _settings.StartingHour - _settings.EndingHour));
                await TryToScheduleQueueForTodayAsync();
            }
        }
        private async Task CheckForActionLimitExceededAsync()
        {
            if(ActionCount >= AccountSettings.ActionsPerDay)
            {
                var timeToNextDay = DateTime.Today.AddDays(1) - DateTime.Now;
                await Task.Delay(timeToNextDay.Milliseconds);
            }
        }

        private async Task TryToScheduleQueueForTodayAsync()
        {
            try
            {
                await _scheduleService.SchedulePromotionQueueForTodayAsync(Profile.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogError($"Could not schedule promotion queue for the current day. " +
                    $"Most likely no schedule was specified.", Profile);
            }
        }
    }
}
