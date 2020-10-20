using FollowUP.Infrastructure.DTO;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IAccountSettingsService : IService
    {
        Task<AccountSettingsDto> GetAccountsSettingsAsync(Guid accountId);
        Task UpdateAccountSettingsAsync(Guid accountId, Guid userId, AccountSettingsDto settings);
    }
}
