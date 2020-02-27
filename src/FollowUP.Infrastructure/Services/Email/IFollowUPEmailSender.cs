using FollowUP.Infrastructure.DTO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    // Marker interface
    public interface IFollowUPEmailSender : IService
    {
        Task<EmailResponseDTO> SendUserVerificationEmailAsync(string displayName, string email, string verificationUrl);
    }
}
