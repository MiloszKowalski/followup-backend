using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryInstagramAccountRepository : IInstagramAccountRepository
    {
        private static readonly ISet<InstagramAccount> _accounts = new HashSet<InstagramAccount>();

        public async Task<IEnumerable<InstagramAccount>> GetAllAsync()
            => await Task.FromResult(_accounts);

        public async Task<InstagramAccount> GetAsync(Guid id)
            => await Task.FromResult(_accounts.SingleOrDefault(x => x.Id == id));

        public async Task<InstagramAccount> GetAsync(string username)
            => await Task.FromResult(_accounts.SingleOrDefault(x => x.Username == username));

        public async Task<IEnumerable<InstagramAccount>> GetUsersAccountsAsync(Guid userId)
            => await Task.FromResult(_accounts.Where(x => x.UserId == userId));

        public async Task AddAsync(InstagramAccount account)
        {
            _accounts.Add(account);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(InstagramAccount account)
        {
            await RemoveAsync(account.Id);
            await AddAsync(account);
        }

        public async Task RemoveAsync(Guid id)
        {
            var account = await GetAsync(id);
            _accounts.Remove(account);
            await Task.CompletedTask;
        }
    }
}
