using FollowUP.Infrastructure.DTO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IEmailSender
    {
        Task<EmailResponseDTO> SendEmailAsync(EmailDetailsDTO details);
    }
}
