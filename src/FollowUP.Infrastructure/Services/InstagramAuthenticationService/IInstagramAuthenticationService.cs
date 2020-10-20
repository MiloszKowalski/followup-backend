using FollowUP.Core.Domain;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.InstagramAuthenticationService
{
    public interface IInstagramAuthenticationService : IService
    {
        Task LoginAsync(InstagramAccount account, string phoneNumber, string twoFactorCode,
                        string verificationCode, bool preferSMSVerification, bool replayChallenge);
    }
}
