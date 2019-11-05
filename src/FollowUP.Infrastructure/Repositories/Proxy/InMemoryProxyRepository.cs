using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryProxyRepository : IProxyRepository
    {
        private static readonly List<InstagramProxy> _proxies = new List<InstagramProxy>();
        private static readonly List<AccountProxy> _accountProxies = new List<AccountProxy>();

        public async Task<IEnumerable<InstagramProxy>> GetAllAsync()
            => await Task.FromResult(_proxies);

        public async Task<InstagramProxy> GetAsync(Guid id)
            => await Task.FromResult(_proxies.SingleOrDefault(x => x.Id == id));

        public async Task<AccountProxy> GetAccountsProxyAsync(Guid accountId)
            => await Task.FromResult(_accountProxies.FirstOrDefault(x => x.AccountId == accountId));

        public async Task<IEnumerable<AccountProxy>> GetProxiesAccountsAsync(Guid proxyId)
           => await Task.FromResult(_accountProxies.Where(x => x.ProxyId == proxyId).ToList());

        public async Task AddAsync(InstagramProxy proxy)
        {
            _proxies.Add(proxy);
            await Task.CompletedTask;
        }

        public async Task AddAccountsProxyAsync(AccountProxy accountProxy)
        {
            _accountProxies.Add(accountProxy);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(InstagramProxy proxy)
        {
            await RemoveAsync(proxy.Id);
            await AddAsync(proxy);
        }

        public async Task UpdateAccountsProxyAsync(AccountProxy accountProxy)
        {
            await RemoveAccountProxyAsync(accountProxy.AccountId);
            await AddAccountsProxyAsync(accountProxy);
        }

        public async Task RemoveAsync(Guid id)
        {
            var proxy = await GetAsync(id);
            _proxies.Remove(proxy);
            await Task.CompletedTask;
        }

        public async Task RemoveAccountProxyAsync(Guid accountId)
        {
            var proxy = await GetAccountsProxyAsync(accountId);
            _accountProxies.Remove(proxy);
            await Task.CompletedTask;
        }
    }
}
