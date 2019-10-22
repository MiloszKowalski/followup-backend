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
        Task AddAsync(InstagramAccount account);
        Task UpdateAsync(InstagramAccount account);
        Task RemoveAsync(Guid id);
    }
}
