using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface ICommentService : IService
    {
        Task<IEnumerable<CommentDto>> GetAllByAccountId(Guid userId);
        Task<IEnumerable<CommentDto>> GetByAccountId(Guid accountId, int page, int pageSize);
        Task<int> GetCount(Guid accountId);
        Task UpdateAllByAccountId(Guid accountId);
    }
}
