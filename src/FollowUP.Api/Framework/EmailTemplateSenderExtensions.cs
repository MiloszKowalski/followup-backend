using FollowUP.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FollowUP.Api.Framework
{
    /// <summary>
    /// Extension methods for any EmailTemplateSender classes
    /// </summary>
    public static class EmailTemplateSenderExtensions
    {
        /// <summary>
        /// Injects the <see cref="EmailTemplateSender"/> into the services to handle the 
        /// <see cref="IEmailTemplateSender"/> service
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to inject to</param>
        /// <returns>The <see cref="IServiceCollection"/> to inject to</returns>
        public static IServiceCollection AddEmailTemplateSender(this IServiceCollection services)
        {
            // Inject the SendGridEmailSender
            services.AddTransient<IEmailTemplateSender, EmailTemplateSender>();

            // Return collection for chaining
            return services;
        }
    }
}
