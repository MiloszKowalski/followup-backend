using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IInstagramAccountRepository : IRepository
    {
        Task<IEnumerable<InstagramAccount>> GetAllAsync();
        Task<InstagramAccount> GetAsync(Guid Id);
        Task<InstagramAccount> GetAsync(string username);
        Task<IEnumerable<InstagramAccount>> GetUsersAccountsAsync(Guid userId);
        Task<IEnumerable<InstagramAccount>> GetAllWithCommentsAsync();
        Task<IEnumerable<InstagramAccount>> GetAllWithPromotionsAsync();
        Task<AccountSettings> GetAccountSettingsAsync(Guid accountId);
        Task AddAccountSettingsAsync(AccountSettings settings);
        Task UpdateAccountSettingsAsync(AccountSettings settings);
        Task RemoveAccountSettingsAsync(Guid accountId);
        Task AddAsync(InstagramAccount account);
        Task UpdateAsync(InstagramAccount account);
        Task RemoveAsync(Guid id);
    }
}
