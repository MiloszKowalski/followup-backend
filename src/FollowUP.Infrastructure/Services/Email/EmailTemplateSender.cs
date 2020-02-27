using FollowUP.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    /// <summary>
    /// Handles sending templated emails
    /// </summary>
    public class EmailTemplateSender : IEmailTemplateSender
    {
        private readonly IEmailSender _emailSender;
        public EmailTemplateSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<EmailResponseDTO> SendGeneralEmailAsync(EmailDetailsDTO details, string title, string content1, string content2, string buttonText, string buttonUrl)
        {
            // Get the templateText from Resources
            var templateText = Properties.Resources.GeneralTemplate;

            // Replace special values with those inside the template
            templateText = templateText.Replace("--Title--", title)
                                        .Replace("--Content1--", content1)
                                        .Replace("--Content2--", content2)
                                        .Replace("--ButtonText--", buttonText)
                                        .Replace("--ButtonUrl--", buttonUrl);

            // Set the details content to this template content
            details.Content = templateText;

            // Send email
            return await _emailSender.SendEmailAsync(details);
        }
    }
}
