using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface ITokenManager : IService
    {
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
    }
}
