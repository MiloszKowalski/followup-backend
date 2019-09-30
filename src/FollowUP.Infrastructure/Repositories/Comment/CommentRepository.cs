using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository, ISqlRepository
    {
        private readonly FollowUPContext _context;

        public CommentRepository(FollowUPContext context)
        {
            _context = context;
        }

        public async Task<Comment> GetAsync(Guid id)
            => await _context.Comments.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId)
            => await _context.Comments.Where(x => x.AccountId == accountId).ToListAsync();

        public async Task<IEnumerable<Comment>> GetAccountCommentsAsync(Guid accountId, int page, int pageSize)
            => await _context.Comments.Where(x => x.AccountId == accountId).Page(page, pageSize).ToListAsync();

        public async Task<int> GetAccountCommentsCountAsync(Guid accountId)
            => await _context.Comments.Where(x => x.AccountId == accountId).CountAsync();

        public async Task<IEnumerable<Comment>> GetAllAsync()
            => await _context.Comments.ToListAsync();

        public async Task<IEnumerable<ChildComment>> GetChildCommentsAsync(Guid commentId)
            => await _context.ChildComments.Where(x => x.ParentCommentId == commentId).ToListAsync();

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task AddChildCommentAsync(ChildComment comment)
        {
            await _context.ChildComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ClearByAccount(Guid accountId)
        {
            var comments = await GetAccountCommentsAsync(accountId);

            if (comments.Count() == 0)
                return;
            foreach(var comment in comments)
            {
                var childComments = await GetChildCommentsAsync(comment.Id);

                foreach(var child in childComments)
                {
                    _context.ChildComments.Remove(child);
                }
                _context.Comments.Remove(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var comment = await GetAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
