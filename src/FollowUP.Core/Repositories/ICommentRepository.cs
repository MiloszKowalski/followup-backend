using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FollowUP.Core.Domain;

namespace FollowUP.Core.Repositories
{
    public interface ICommentRepository : IRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment> GetAsync(Guid Id);
        Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid userId);
        Task<IEnumerable<ChildComment>> GetChildCommentsAsync(Guid commentId);
        Task AddAsync(Comment comment);
        Task AddChildCommentAsync(ChildComment comment);
        Task UpdateAsync(Comment comment);
        Task RemoveAsync(Guid id);
        Task ClearByAccount(Guid accountId);
    }
}
