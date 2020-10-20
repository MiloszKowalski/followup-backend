using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface ITokenManager : IService
    {
        Task<bool> IsCurrentActiveTokenAsync();
        Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
    }
}
