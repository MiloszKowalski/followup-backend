using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace FollowUP.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowUPEmailSender _emailSender;
        private readonly ApiSettings _apiSettings;
        private readonly IJwtHandler _jwtHandler;
        private readonly IEncryptor _encrypter;
        private readonly IMapper _mapper;
        private readonly EmailSettings _emailSettings;

        public UserService(IUserRepository userRepository, IFollowUPEmailSender emailSender,
            ApiSettings apiSettings, IJwtHandler jwtHandler, IEncryptor encrypter,
            IMapper mapper, EmailSettings emailSettings)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _apiSettings = apiSettings;
            _jwtHandler = jwtHandler;
            _encrypter = encrypter;
            _mapper = mapper;
            _emailSettings = emailSettings;
        }

        public async Task<UserDto> GetAsync(string email)
        {
            var user = await _userRepository.GetAsync(email);

            return _mapper.Map<User, UserDto>(user);
        }

        public async Task<UserDto> GetAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);

            return _mapper.Map<User, UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> BrowseAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users);
        }

        public async Task LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetAsync(email);
            if (user == null)
            {
                throw new ServiceException(ErrorCodes.InvalidCredentials,
                    "Invalid credentials");
            }

            var hash = _encrypter.GetHash(password, user.Salt);
            if (user.Password == hash)
            {
                return;
            }

            throw new ServiceException(ErrorCodes.InvalidCredentials,
                "Invalid credentials");
        }

        public async Task RegisterAsync(Guid userId, string email,
            string username, string fullname, string password, string role)
        {
            var user = await _userRepository.GetAsync(email);
            if (user != null)
            {
                throw new ServiceException(ErrorCodes.EmailInUse,
                    $"User with email: '{email}' already exists.");
            }

            var userByUsername = await _userRepository.GetByUsernameAsync(username);
            if(userByUsername != null)
            {
                throw new ServiceException(ErrorCodes.UsernameInUse,
                    $"User with username: '{username}' already exists.");
            }

            var salt = _encrypter.GetSalt(password);
            var hash = _encrypter.GetHash(password, salt);
            user = new User(userId, email, username, fullname, role, hash, salt);
            await _userRepository.AddAsync(user);

            // Generate verification url
            var verificationToken = _encrypter.GetHash(username, salt);
            var verificationUrl = $"https://{_apiSettings.DashboardBaseUrl}/" +
                                $"{HttpUtility.UrlEncode(userId.ToString())}/" +
                                $"{HttpUtility.UrlEncode(verificationToken)}/";

            // Send confirmation email
            if (_emailSettings.SendVerificationMail)
            {
                await _emailSender.SendUserVerificationEmailAsync(username, email, verificationUrl);
            }
        }

        public async Task<JwtDto> RefreshAccessTokenAsync(string token)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);
            if (refreshToken == null)
            {
                throw new ServiceException("Refresh token was not found.");
            }

            // Check if the user exists in database
            var user = await _userRepository.GetAsync(refreshToken.UserId);
            if (user == null)
            {
                throw new ServiceException(ErrorCodes.UserNotFound,
                    "User with given id doesn't exist.");
            }

            // Create and return JWT token to the user
            var jwt = _jwtHandler.CreateToken(refreshToken.UserId, user.Role);
            jwt.RefreshToken = refreshToken.Token;

            return jwt;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);
            if (refreshToken == null)
            {
                throw new ServiceException("Refresh token was not found.");
            }

            await _userRepository.RemoveRefreshTokenAsync(token);
        }

        public async Task ConfirmEmailTokenAsync(Guid userId, string registrationToken)
        {
            var user = await _userRepository.GetAsync(userId);

            if (user == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    "Confirmation link was not valid. Please contact FollowUP support " +
                    "at biuro@followup.social");
            }

            var hash = _encrypter.GetHash(user.Id.ToString(), user.Salt);

            if (hash != registrationToken)
            {
                throw new ServiceException(ErrorCodes.InvalidToken,
                    "The provided token was not valid.");
            }

            user.SetVerified(true);
            await _userRepository.UpdateAsync(user);
        }
    }
}
