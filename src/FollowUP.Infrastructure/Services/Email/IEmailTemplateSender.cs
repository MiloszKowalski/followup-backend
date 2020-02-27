using FollowUP.Infrastructure.DTO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IEmailTemplateSender
    {
        Task<EmailResponseDTO> SendGeneralEmailAsync(EmailDetailsDTO details, string title, string content1, string content2, string buttonText, string buttonUrl);
    }
}
