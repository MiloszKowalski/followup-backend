using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class InstagramAccountService : IInstagramAccountService
    {
        private readonly IInstagramAccountRepository _instagramAccountRepository;
        private readonly IInstagramApiService _instagramApiService;
        private readonly IMapper _mapper;

        public InstagramAccountService(IInstagramAccountRepository instagramAccountRepository,
                IMapper mapper, IInstagramApiService instagramApiService)
        {
            _instagramAccountRepository = instagramAccountRepository;
            _instagramApiService = instagramApiService;
            _mapper = mapper;;
        }

        /// <summary>
        /// Gets the instagram accounts (for admin purposes)
        /// </summary>
        /// <param name="page">The page to display</param>
        /// <param name="pageSize">The number of elements to display on each page</param>
        /// <returns>List of the users paginated</returns>
        public async Task<IEnumerable<InstagramAccountDto>> GetAsync(int page, int pageSize)
        {
            // Get the accounts from repository
            var accounts = await _instagramAccountRepository.GetAllWithCompleteInfoAsync(page, pageSize);

            return _mapper.Map<IEnumerable<InstagramAccount>, IEnumerable<AdminExtendedAccountDto>>(accounts);
        }

        /// <summary>
        /// Gets the instagram accounts count (for admin purposes)
        /// </summary>
        /// <returns>Count of the accounts</returns>
        public async Task<int> GetCountAsync()
        {
            // Get the accounts count
            var count = await _instagramAccountRepository.GetCountAsync();
            return count;
        }

        /// <summary>
        /// Gets all the accounts for given user ID
        /// </summary>
        /// <param name="userId">ID of the user that owns the accounts</param>
        /// <returns>List of Accounts that the user owns</returns>
        public async Task<IEnumerable<InstagramAccountDto>> GetAllByUserIdAsync(Guid userId)
        {
            // Get the accounts from repository
            var accounts = await _instagramAccountRepository.GetUsersAccountsAsync(userId);

            // Map the accounts and return them as a list of AccountDTO
            return _mapper.Map<IEnumerable<InstagramAccount>, IEnumerable<InstagramAccountDto>>(accounts);
        }

        /// <summary>
        /// Gets the account with extended information for given account ID
        /// </summary>
        /// <param name="accountId">ID of the account to search for</param>
        /// <returns>Account extended info</returns>
        public async Task<InstagramAccountDto> GetExtendedInfoByIdAsync(Guid accountId)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            var slaveAccount = await _instagramApiService.GetRandomSlaveAccountAsync();

            var instaApi = await _instagramApiService.GetInstaApiAsync(slaveAccount);
            if (instaApi == null)
            {
                return null;
            }

            if (!instaApi.IsUserAuthenticated)
            {
                return null;
            }

            var accountInfo = await instaApi.UserProcessor.GetUserInfoByUsernameAsync(account.Username);

            var extendedAccount = _mapper.Map<InstagramAccount, ExtendedAccountDto>(account);

            if (accountInfo.Succeeded)
            {
                extendedAccount.FollowersCount = accountInfo.Value.FollowerCount;
                extendedAccount.FollowingCount = accountInfo.Value.FollowingCount;
                extendedAccount.ProfilePictureUrl = accountInfo.Value.ProfilePicUrl;
                extendedAccount.ProfileName = accountInfo.Value.FullName;
            }

            return extendedAccount;
        }

        /// <summary>
        /// Gets all the accounts with extended information for given user ID
        /// </summary>
        /// <param name="userId">ID of the user that owns the accounts</param>
        /// <returns>List of Accounts that the user owns</returns>
        public async Task<IEnumerable<InstagramAccountDto>> GetAllByUserIdExtendedAsync(Guid userId)
        {
            var accounts = await _instagramAccountRepository.GetUsersAccountsAsync(userId);

            var extendedAccounts = new List<ExtendedAccountDto>();

            var slaveAccount = await _instagramApiService.GetRandomSlaveAccountAsync();

            var instaApi = await _instagramApiService.GetInstaApiAsync(slaveAccount);
            if (instaApi == null)
            {
                return null;
            }

            if (!instaApi.IsUserAuthenticated)
            {
                return null;
            }

            foreach (var account in accounts)
            { 
                var accountInfo = await instaApi.UserProcessor.GetUserInfoByUsernameAsync(account.Username);

                var extendedAccount = _mapper.Map<InstagramAccount, ExtendedAccountDto>(account);

                if(accountInfo.Succeeded)
                {
                    extendedAccount.FollowersCount = accountInfo.Value.FollowerCount;
                    extendedAccount.FollowingCount = accountInfo.Value.FollowingCount;
                    extendedAccount.ProfilePictureUrl = accountInfo.Value.ProfilePicUrl;
                    extendedAccount.ProfileName = accountInfo.Value.FullName;
                }

                extendedAccounts.Add(extendedAccount);
            }
            
            return extendedAccounts;
        }

        public async Task BuyCommentsAsync(Guid accountId, double daysToAdd)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            if (daysToAdd <= 0)
            {
                throw new ServiceException(ErrorCodes.DaysNotPositive,
                    "Days to add must be a positive number");
            }

            account.BuyComments(daysToAdd);

            await _instagramAccountRepository.UpdateAsync(account);
        }

        public async Task BuyPromotionsAsync(Guid accountId, double daysToAdd)
        {
            if (daysToAdd <= 0)
            {
                throw new ServiceException(ErrorCodes.DaysNotPositive,
                    "Days to add must be a positive number.");
            }

            var account = await _instagramAccountRepository.GetAsync(accountId);
            if (account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist,
                    $"Cannot find account with given id: {accountId}.");
            }

            account.BuyPromotions(daysToAdd);

            await _instagramAccountRepository.UpdateAsync(account);
        }
    }
}
