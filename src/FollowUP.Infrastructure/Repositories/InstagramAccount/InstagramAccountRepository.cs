using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InstagramAccountRepository : IInstagramAccountRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public InstagramAccountRepository(FollowUPContext context)
        {
            _context = context;
        }

        public async Task<InstagramAccount> GetAsync(Guid id)
            => await _context.InstagramAccounts.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<InstagramAccount> GetAsync(string username)
            => await _context.InstagramAccounts.SingleOrDefaultAsync(x => x.Username == username);

        public async Task<IEnumerable<InstagramAccount>> GetUsersAccountsAsync(Guid userId)
            => await _context.InstagramAccounts.Where(x => x.UserId == userId).ToListAsync();

        public async Task<IEnumerable<InstagramAccount>> GetAllAsync()
            => await _context.InstagramAccounts.ToListAsync();

        public async Task AddAsync(InstagramAccount account)
        {
            await _context.InstagramAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InstagramAccount account)
        {
            _context.InstagramAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var account = await GetAsync(id);
            _context.InstagramAccounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}
