using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class ProxyRepository : IProxyRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public ProxyRepository(FollowUPContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InstagramProxy>> GetAllAsync()
            => await _context.InstagramProxies.ToListAsync();

        public async Task<InstagramProxy> GetAsync(Guid id)
            => await _context.InstagramProxies.SingleOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(InstagramProxy proxy)
        {
            await _context.InstagramProxies.AddAsync(proxy);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InstagramProxy proxy)
        {
            _context.InstagramProxies.Update(proxy);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var proxy = await GetAsync(id);
            _context.InstagramProxies.Remove(proxy);
            await _context.SaveChangesAsync();
        }
    }
}
