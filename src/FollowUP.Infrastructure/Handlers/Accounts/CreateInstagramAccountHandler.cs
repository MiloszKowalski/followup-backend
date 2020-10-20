using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Factories;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Services.InstagramAuthenticationService;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers
{
    public class CreateInstagramAccountHandler : ICommandHandler<CreateInstagramAccount>
    {
        private readonly IAesEncryptor _encryptor;
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IInstagramAuthenticationService _instagramAuthService;
        private readonly IInstagramApiService _instagramApiService;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly PromotionSettings _settings;
        private readonly IUserRepository _userRepository;

        public CreateInstagramAccountHandler(
            IInstagramAuthenticationService instagramAuthService,
            IUserRepository userRepository,
            IInstagramAccountRepository accountRepository,
            PromotionSettings settings,
            IProxyRepository proxyRepository,
            IInstagramApiService instagramApiService,
            IPromotionRepository promotionRepository,
            IAesEncryptor encryptor)
        {
            _accountRepository = accountRepository;
            _encryptor = encryptor;
            _instagramAuthService = instagramAuthService;
            _instagramApiService = instagramApiService;
            _promotionRepository = promotionRepository;
            _proxyRepository = proxyRepository;
            _settings = settings;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(CreateInstagramAccount c)
        {
            var accountId = Guid.NewGuid();
            var account = await PrepareAccountForLoginAsync(accountId, c);
            await _instagramAuthService.LoginAsync(account, c.PhoneNumber, c.TwoFactorCode,
                                                      c.VerificationCode, c.PreferSMSVerification, c.ReplayChallenge);

            InstagramProxy accountProxy = null;
            if (_settings.UseProxy)
            {
                accountProxy = await PrepareAccountProxyAsync(accountId);
            }

            // Store the account in database
            await _accountRepository.AddAsync(account);

            // Create single unfollow promotion for scheduling purposes
            var unfollowPromotion = PromotionFactory.GetUnfollowPromotion(account.Id);
            await _promotionRepository.AddAsync(unfollowPromotion);

            if (_settings.UseProxy)
            {
                await _proxyRepository.AddAsync(accountProxy);
            }

            // Create and store account's settings
            var accountSettings = AccountSettingsFactory.GetDefaultAccountSettings(accountId);
            await _accountRepository.AddAccountSettingsAsync(accountSettings);
        }

        /// <summary>
        /// Create Instagram account for further authentication
        /// </summary>
        /// <param name="c"><see cref="CreateInstagramAccount"/> command for the account to be created</param>
        private async Task<InstagramAccount> PrepareAccountForLoginAsync(Guid accountId, CreateInstagramAccount c)
        {
            var user = await _userRepository.GetAsync(c.UserId);
            if (user == null)
            {
                throw new ServiceException(ErrorCodes.UserNotFound,
                    "Cannot find desired user (not authenticated?)");
            }

            var androidDevice = AndroidDeviceGenerator.GetRandomName();

            var slaveAccount = await _instagramApiService.GetRandomSlaveAccountAsync();

            var instaApi = await _instagramApiService.GetInstaApiAsync(slaveAccount);
            if (instaApi == null)
            {
                throw new ServiceException(ErrorCodes.ServiceUnavailable,
                    "No slave account available for the Pk retrieval.");
            }

            var userInfo = await instaApi.UserProcessor.GetUserAsync(c.Username);
            if (!userInfo.Succeeded)
            {
                throw new ServiceException(ErrorCodes.UserNotFound,
                    "Cannot find profile with given username.");
            }

            var pk = userInfo.Value.Pk;

            // Check if the account is already created, if yes, don't create a new one
            // (as instagram accounts are unique and two users can't have the same one)
            var instagramAccountTest = await _accountRepository.GetAsync(pk.ToString());
            if (instagramAccountTest != null)
            {
                if (instagramAccountTest.User.Id != c.UserId)
                {
                    throw new ServiceException(ErrorCodes.AccountAlreadyExists,
                        "Account is already taken by another user");
                }
            }

            var encryptedPassword = _encryptor.Encrypt(c.Password);

            return new InstagramAccount(accountId, c.UserId, pk.ToString(), user.Username,
                c.Username, encryptedPassword, androidDevice);
        }

        private async Task<InstagramProxy> PrepareAccountProxyAsync(Guid accountId)
        {
            InstagramProxy proxy = null;

            var proxies = await _proxyRepository.GetAllAsync();
            proxy = proxies.FirstOrDefault
            (
                x => x.ExpiryDate.ToUniversalTime() > DateTime.UtcNow && !x.IsTaken
            );

            if (proxy == null)
            {
                throw new ServiceException(ErrorCodes.NoProxyAvailable,
                    "There is no proxy available for account creation.");
            }

            proxy.SetIsTaken(true);
            proxy.InstagramAccountId = accountId;
            await _proxyRepository.UpdateAsync(proxy);

            return proxy;
        }
    }
}
