using FollowUP.Core.Domain;
using InstagramApiSharp.API;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramApiService : IService
    {
        Task<IInstaApi> GetInstaApi(InstagramAccount account, bool forLogin = false);
        Task<InstagramAccount> GetRandomSlaveAccount();
        Task SendColdStartMockupRequests(IInstaApi instaApi);
        Task GetUserProfileMockAsync(IInstaApi instaApi);
        Task GetUserFollowedAsync(IInstaApi instaApi, InstagramAccount account);
        Task UnfollowUsersAsync(IInstaApi instaApi, InstagramAccount account, int count);
    }
}
