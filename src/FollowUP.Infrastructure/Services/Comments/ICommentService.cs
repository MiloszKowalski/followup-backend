using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface ICommentService : IService
    {
        Task<IEnumerable<CommentDto>> GetAllRecentByAccountId(Guid userId);
        Task<bool> UpdateAllRecentByAccountId(Guid userId);
    }
}
