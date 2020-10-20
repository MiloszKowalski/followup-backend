using AutoMapper;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Settings;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.AccountSettingsService
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly PromotionSettings _settings;
        private readonly IMapper _mapper;

        public AccountSettingsService(IMapper mapper,
            IInstagramAccountRepository accountRepository,
            PromotionSettings settings)
        {
            _accountRepository = accountRepository;
            _settings = settings;
            _mapper = mapper;
        }

        public async Task<AccountSettingsDto> GetAccountsSettingsAsync(Guid accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Account with given ID doesn't exist yet.");
            }

            var settings = await _accountRepository.GetAccountSettingsAsync(accountId);

            return _mapper.Map<AccountSettingsDto>(settings);
        }

        public async Task UpdateAccountSettingsAsync(Guid accountId,
            Guid userId, AccountSettingsDto settings)
        {
            var account = await _accountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Account with ID: {accountId} doesn't exist.");
            }

            if (account.User.Id != userId)
            {
                throw new ServiceException(ErrorCodes.UserNotPermitted,
                    "Cannot change settings of an account that doesn't belong to your account.");
            }

            var currentSettings = await _accountRepository.GetAccountSettingsAsync(accountId);

            if (currentSettings == null)
            {
                throw new ServiceException(ErrorCodes.SettingsNotFound,
                    $"Couldn't find settings for given account ID: {accountId}");
            }

            if (settings.MaxIntervalMilliseconds < settings.MinIntervalMilliseconds)
            {
                throw new ServiceException(ErrorCodes.MaxLessThanMin,
                    "Maximum interval milliseconds value is less its minimum value.");
            }

            if (settings.MinIntervalMilliseconds < 0
                || settings.MinIntervalMilliseconds > _settings.MinActionIntervalLimit)
            {
                throw new ServiceException(ErrorCodes.InvalidMinIntervalMilliseconds,
                    "Minimum interval milliseconds value is beyond its allowed limits.");
            }

            if (settings.MaxIntervalMilliseconds < 0
                || settings.MaxIntervalMilliseconds > _settings.MaxActionIntervalLimit)
            {
                throw new ServiceException(ErrorCodes.InvalidMinIntervalMilliseconds,
                    "Maximum interval milliseconds value is beyond its allowed limits.");
            }

            currentSettings.SetActionsPerDay(settings.ActionsPerDay);
            currentSettings.SetFollowsPerDay(settings.FollowsPerDay);
            currentSettings.SetLikesPerDay(settings.LikesPerDay);
            currentSettings.SetUnfollowsPerDay(settings.UnfollowsPerDay);
            currentSettings.SetMinIntervalMilliseconds(settings.MinIntervalMilliseconds);
            currentSettings.SetMaxIntervalMilliseconds(settings.MaxIntervalMilliseconds);

            await _accountRepository.UpdateAccountSettingsAsync(currentSettings);
        }
    }
}
