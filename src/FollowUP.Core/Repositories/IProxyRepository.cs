using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface IProxyRepository : IRepository
    {
        Task<IEnumerable<InstagramProxy>> GetAllAsync();
        Task<InstagramProxy> GetAsync(Guid Id);
        Task<AccountProxy> GetAccountsProxyAsync(Guid accountId);
        Task<IEnumerable<AccountProxy>> GetProxiesAccountsAsync(Guid proxyId);
        Task AddAsync(InstagramProxy proxy);
        Task AddAccountsProxyAsync(AccountProxy accountProxy);
        Task UpdateAccountsProxyAsync(AccountProxy accountProxy);
        Task UpdateAsync(InstagramProxy proxy);
        Task RemoveAsync(Guid id);
    }
}
