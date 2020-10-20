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
        Task AddAsync(InstagramProxy proxy);
        Task UpdateAsync(InstagramProxy proxy);
        Task RemoveAsync(Guid id);
    }
}
