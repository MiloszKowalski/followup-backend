using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryInstagramAccountRepository : IInstagramAccountRepository
    {
        private static readonly ISet<InstagramAccount> _accounts = new HashSet<InstagramAccount>();
        private static readonly ISet<AccountSettings> _settings = new HashSet<AccountSettings>();

        public async Task<IEnumerable<InstagramAccount>> GetAllAsync()
            => await Task.FromResult(_accounts);

        public async Task<InstagramAccount> GetAsync(Guid id)
            => await Task.FromResult(_accounts.SingleOrDefault(x => x.Id == id));

        public async Task<InstagramAccount> GetAsync(string instagramPk)
            => await Task.FromResult(_accounts.SingleOrDefault(x => x.Pk == instagramPk));

        public async Task<IEnumerable<InstagramAccount>> GetAsync(int page, int pageSize)
            => await Task.FromResult(_accounts.Page(page, pageSize));

        public Task<IEnumerable<InstagramAccount>> GetAllWithCompleteInfoAsync(int page, int pageSize)
            => throw new NotImplementedException();

        public async Task<int> GetCountAsync()
            => await Task.FromResult(_accounts.Count());

        public async Task<InstagramAccount> GetByUsernameAsync(string username)
            => await Task.FromResult(_accounts.SingleOrDefault(x => x.Username == username));

        public async Task<IEnumerable<InstagramAccount>> GetUsersAccountsAsync(Guid userId)
            => await Task.FromResult(_accounts.Where(x => x.User.Id == userId));

        public async Task<IEnumerable<InstagramAccount>> GetAllWithCommentsAsync()
            => await Task.FromResult(_accounts.Where(x => x.CommentsModuleExpiry > DateTime.UtcNow));
        public async Task<IEnumerable<InstagramAccount>> GetAllWithPromotionsAsync()
            => await Task.FromResult(_accounts.Where(x => x.PromotionsModuleExpiry > DateTime.UtcNow));
        public async Task<AccountSettings> GetAccountSettingsAsync(Guid accountId)
            => await Task.FromResult(_settings.SingleOrDefault(x => x.InstagramAccountId == accountId));

        public async Task AddAsync(InstagramAccount account)
        {
            _accounts.Add(account);
            await Task.CompletedTask;
        }

        public async Task AddAccountSettingsAsync(AccountSettings settings)
        {
            _settings.Add(settings);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(InstagramAccount account)
        {
            await RemoveAsync(account.Id);
            await AddAsync(account);
        }

        public async Task UpdateAccountSettingsAsync(AccountSettings settings)
        {
            await RemoveAccountSettingsAsync(settings.Id);
            await AddAccountSettingsAsync(settings);
        }

        public async Task RemoveAccountSettingsAsync(Guid id)
        {
            var settings = await GetAccountSettingsAsync(id);
            _settings.Remove(settings);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(Guid id)
        {
            var account = await GetAsync(id);
            _accounts.Remove(account);
            await Task.CompletedTask;
        }
    }
}
