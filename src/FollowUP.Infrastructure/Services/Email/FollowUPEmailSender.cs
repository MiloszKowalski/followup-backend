using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Settings;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    /// <summary>
    /// Handles sending emails specific to the FollowUP Word server
    /// </summary>
    public class FollowUPEmailSender : IFollowUPEmailSender
    {
        private readonly IEmailTemplateSender _emailTemplateSender;
        private readonly EmailSettings _settings;

        public FollowUPEmailSender(IEmailTemplateSender emailTemplateSender, EmailSettings settings)
        {
            _emailTemplateSender = emailTemplateSender;
            _settings = settings;
        }

        /// <summary>
        /// Sends a verification email to the specified user
        /// </summary>
        /// <param name="displayName">The users display name (typically first name)</param>
        /// <param name="email">The users email to be verified</param>
        /// <param name="verificationUrl">The URL the user needs to click to verify their email</param>
        /// <returns></returns>
        public async Task<EmailResponseDTO> SendUserVerificationEmailAsync(string displayName, string email, string verificationUrl)
        {
            return await _emailTemplateSender.SendGeneralEmailAsync(new EmailDetailsDTO
            {
                IsHTML = true,
                FromEmail = _settings.SendFromEmail,
                FromName = _settings.SendFromName,
                ToEmail = email,
                ToName = displayName,
                Subject = "Zweryfikuj swój adres e-mail"
            },
            "Zweryfikuj adres e-mail",
            $"Cześć {displayName ?? "gwiazdo"},",
            "Dziękujemy za założenie konta w naszym serwisie.<br/>" +
            "Prosimy o zweryfikowanie przypisanego do niego adresu e-mail, klikając w przycisk poniżej.",
            "Zweryfikuj e-mail",
            verificationUrl
            );
        }
    }
}
