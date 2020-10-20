using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IInstagramAccountRepository : IRepository
    {
        Task<IEnumerable<InstagramAccount>> GetAllAsync();
        Task<IEnumerable<InstagramAccount>> GetAsync(int page, int pageSize);
        Task<IEnumerable<InstagramAccount>> GetAllWithCompleteInfoAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        Task<InstagramAccount> GetAsync(Guid id);
        Task<InstagramAccount> GetAsync(string instagramPk);
        Task<InstagramAccount> GetByUsernameAsync(string username);
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
