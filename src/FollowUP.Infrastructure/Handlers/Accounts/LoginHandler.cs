using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Accounts;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class LoginHandler : ICommandHandler<Login>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMemoryCache _cache;
        
        public LoginHandler(IUserRepository userRepository, IUserService userService, IJwtHandler jwtHandler,
            IMemoryCache cache, IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _userService = userService;
            _jwtHandler = jwtHandler;
            _cache = cache;
        }

        public async Task HandleAsync(Login command)
        {
            await _userService.LoginAsync(command.Email, command.Password);
            var user = await _userRepository.GetAsync(command.Email);
            var jwt = _jwtHandler.CreateToken(user.Id, user.Role);

            var refreshToken = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                .Replace("+", string.Empty)
                .Replace("=", string.Empty)
                .Replace("/", string.Empty);

            var refreshTokenCheck = await _userRepository.GetDeviceRefreshToken(user.Id, command.UserAgent);

            if(refreshTokenCheck == null)
            {
                await _userRepository.AddRefreshToken(
                    new RefreshToken
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Token = refreshToken,
                        UserAgent = command.UserAgent
                    });

                jwt.RefreshToken = refreshToken;
            }
            else if (refreshTokenCheck.Revoked)
            {
                refreshTokenCheck.Token = refreshToken;
                refreshTokenCheck.Revoked = false;

                await _userRepository.UpdateRefreshToken(refreshTokenCheck);

                jwt.RefreshToken = refreshToken;
            }
            else
                jwt.RefreshToken = refreshTokenCheck.Token;


            _cache.SetJwt(command.TokenId, jwt);
        }
    }
}
