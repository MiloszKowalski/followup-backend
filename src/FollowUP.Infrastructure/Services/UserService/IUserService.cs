using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IUserService : IService
    {
        Task<UserDto> GetAsync(string email);
        Task<UserDto> GetAsync(Guid userId);
        Task<IEnumerable<UserDto>> BrowseAsync();
        Task RegisterAsync(Guid userId, string email,
            string username, string fullname, string password, string role);
        Task LoginAsync(string email, string password);
        Task<JwtDto> RefreshAccessTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task ConfirmEmailTokenAsync(Guid userId, string registrationToken);
    }
}
