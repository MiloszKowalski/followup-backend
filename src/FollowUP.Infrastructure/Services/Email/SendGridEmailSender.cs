using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Settings;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        public SendGridEmailSender(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task<EmailResponseDTO> SendEmailAsync(EmailDetailsDTO details)
        {
            // Get the SendGrid key
            var apiKey = _settings.SendGridKey;

            // Create a new SendGrid client
            var client = new SendGridClient(apiKey);

            // From
            var from = new EmailAddress(details.FromEmail, details.FromName);

            // To
            var to = new EmailAddress(details.ToEmail, details.ToName);

            // Subject
            var subject = details.Subject;

            // Content
            var content = details.Content;

            // Create Email class ready to send
            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                // Plain content
                details.IsHTML ? null : details.Content,
                // HTML content
                details.IsHTML ? details.Content : null);

            // Finally, send the email...
            var response = await client.SendEmailAsync(msg);

            // If we succeeded...
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                // Return successful response
                return new EmailResponseDTO();

            // Otherwise, it failed...

            try
            {
                // Get the result in the body
                var bodyResult = await response.Body.ReadAsStringAsync();

                // Deserialize the response
                var sendGridResponse = JsonConvert.DeserializeObject<SendGridResponse>(bodyResult);

                // Add any errors to the response
                var errorResponse = new EmailResponseDTO
                {
                    Errors = sendGridResponse?.Errors.Select(f => f.Message).ToList()
                };

                // Make sure we have at least one error
                if (errorResponse.Errors == null || errorResponse.Errors.Count == 0)
                    // Add an unknown error
                    errorResponse.Errors = new List<string>(new[] { "Unknown error from email sending service. Please contact FollowUP support." });

                // Return the response
                return errorResponse;

            }
            catch (Exception ex)
            {
                // TODO: Localization

                // Break if we are debugging
                if (Debugger.IsAttached)
                {
                    var error = ex;
                    Debugger.Break();
                }

                // If something unexpected happened, return message
                return new EmailResponseDTO
                {
                    Errors = new List<string>(new[] { "Unknown error occurred" })
                };
            }
        }
    }
}
