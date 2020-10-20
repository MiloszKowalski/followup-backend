using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private static readonly ISet<User> _users = new HashSet<User>();
        private static readonly ISet<RefreshToken> _refreshTokens = new HashSet<RefreshToken>();

        public async Task<User> GetAsync(Guid id)
            => await Task.FromResult(_users.SingleOrDefault(x => x.Id == id));

        public async Task<User> GetAsync(string email)
            => await Task.FromResult(_users.SingleOrDefault(x => x.Email == email.ToLowerInvariant()));

        public async Task<User> GetByUsernameAsync(string username)
            => await Task.FromResult(_users.SingleOrDefault(x => x.Username == username.ToLowerInvariant()));

        public async Task<IEnumerable<User>> GetAllAsync()
            => await Task.FromResult(_users);

        public async Task AddAsync(User user)
        {
            _users.Add(user);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(Guid id)
        {
            var user = await GetAsync(id);
            _users.Remove(user);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(User user)
        {
            await Task.CompletedTask;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
            => await Task.FromResult(_refreshTokens.SingleOrDefault(x => x.Token == token));

        public async Task<RefreshToken> GetDeviceRefreshTokenAsync(Guid userId, string userAgent)
            => await Task.FromResult(_refreshTokens.SingleOrDefault(x => x.UserId == userId && x.UserAgent == userAgent));

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            _refreshTokens.Add(token);
            await Task.CompletedTask;
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            await Task.CompletedTask;
        }

        public async Task RemoveRefreshTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            _refreshTokens.Remove(refreshToken);
            await Task.CompletedTask;
        }
    }
}
