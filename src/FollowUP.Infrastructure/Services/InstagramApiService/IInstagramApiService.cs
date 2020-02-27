using FollowUP.Core.Domain;
using InstagramApiSharp.API;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramApiService : IService
    {
        Task<IInstaApi> GetInstaApi(InstagramAccount account);
        Task<InstagramAccount> GetRandomSlaveAccount();
    }
}
