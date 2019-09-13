using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IDataInitializer : IService
    {
        Task SeedAsync();
    }
}
