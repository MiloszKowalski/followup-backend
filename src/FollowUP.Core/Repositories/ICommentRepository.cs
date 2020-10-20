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
        Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId);
        Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId, int page, int pageSize);
        Task<int> GetAccountCommentsCountAsync(Guid accountId);
        Task<IEnumerable<ChildComment>> GetChildCommentsAsync(Guid commentId);
        Task AddAsync(Comment comment);
        Task AddChildCommentAsync(ChildComment comment);
        Task UpdateAsync(Comment comment);
        Task RemoveAsync(Guid id);
        Task ClearByAccountAsync(Guid accountId);
    }
}
