using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryCommentRepository : ICommentRepository
    {
        private static readonly List<Comment> _comments = new List<Comment>();
        private static readonly List<ChildComment> _childComments = new List<ChildComment>();

        public async Task<IEnumerable<Comment>> GetAllAsync()
            => await Task.FromResult(_comments);

        public async Task<Comment> GetAsync(Guid id)
            => await Task.FromResult(_comments.SingleOrDefault(x => x.Id == id));

        public async Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId)
            => await Task.FromResult(_comments.Where(x => x.AccountId == accountId));

        public async Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId, int page, int pageSize)
            => await Task.FromResult(_comments.Where(x => x.AccountId == accountId).Page(page, pageSize));

        public async Task<int> GetAccountCommentsCountAsync(Guid accountId)
            => await Task.FromResult(_comments.Where(x => x.AccountId == accountId).Count());

        public async Task<IEnumerable<ChildComment>> GetChildCommentsAsync(Guid commentId)
            => await Task.FromResult(_childComments.Where(x => x.ParentCommentId == commentId));

        public async Task AddAsync(Comment comment)
        {
            _comments.Add(comment);
            await Task.CompletedTask;
        }

        public async Task AddChildCommentAsync(ChildComment comment)
        {
            _childComments.Add(comment);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Comment comment)
        {
            await RemoveAsync(comment.Id);
            await AddAsync(comment);
        }

        public async Task RemoveAsync(Guid id)
        {
            var comment = await GetAsync(id);
            _comments.Remove(comment);
            await Task.CompletedTask;
        }

        public async Task ClearByAccount(Guid accountId)
        {
            _comments.Clear();
            _childComments.Clear();
            await Task.CompletedTask;
        }
    }
}
