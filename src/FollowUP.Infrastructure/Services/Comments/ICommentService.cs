using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface ICommentService : IService
    {
        Task<IEnumerable<CommentDto>> GetAllByAccountIdAsync(Guid userId);
        Task<IEnumerable<CommentDto>> GetByAccountIdAsync(Guid accountId, int page, int pageSize);
        Task<int> GetCountAsync(Guid accountId);
        Task UpdateAllByAccountIdAsync(Guid accountId);
    }
}
