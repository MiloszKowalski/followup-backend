using FollowUP.Core.Domain;
using InstagramApiSharp.API;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramApiService : IService
    {
        Task<IInstaApi> GetInstaApi(InstagramAccount account, bool forLogin = false);
        Task<InstagramAccount> GetRandomSlaveAccount();
        Task SendColdStartMockupRequests(IInstaApi instaApi, InstagramAccount account);
        Task GetUserProfileMockAsync(IInstaApi instaApi, InstagramAccount account);
        Task GetUserFollowedAsync(IInstaApi instaApi, InstagramAccount account);
        Task UnfollowUsersAsync(IInstaApi instaApi, InstagramAccount account, int count);
        Task GetHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag);
        Task LikeHashtagMediaAsync(IInstaApi instaApi, InstagramAccount account, string tag, int count);
    }
}
