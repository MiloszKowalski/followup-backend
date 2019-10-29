using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IInstagramAccountService : IService
    {
        Task CreateAsync(Guid Id, Guid userId, string username, string password);
        Task LoginAsync(string username, string password, string phoneNumber, string twoFactorCode,
                        string verificationCode, bool preferSMSVerification, bool replayChallenge);
        Task LoginToEmbeddedBrowserAsync(string username, string password, string twoFactorCode, string verificationCode);
        Task<IEnumerable<AccountDto>> GetAllByUserId(Guid userId);
        Task BuyComments(Guid accountId, double daysToAdd);
        Task BuyPromotions(Guid accountId, double daysToAdd);
    }
}
