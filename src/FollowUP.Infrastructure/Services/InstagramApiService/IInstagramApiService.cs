using FollowUP.Core.Domain;
using InstagramApiSharp.API;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramApiService : IService
    {
        Task<IInstaApi> GetInstaApiAsync(InstagramAccount account, bool forLogin = false);
        Task<InstagramAccount> GetRandomSlaveAccountAsync();
        Task SendColdStartMockupRequestsAsync(IInstaApi instaApi, InstagramAccount account);
        Task GetUserProfileMockAsync(IInstaApi instaApi, InstagramAccount account);
        Task GetUserFollowedAsync(IInstaApi instaApi, InstagramAccount account);
        Task UnfollowUserAsync(IInstaApi instaApi, InstagramAccount account);
        Task GetHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag);
        Task LikeHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag, int count);
    }
}
