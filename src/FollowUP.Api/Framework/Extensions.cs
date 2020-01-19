using Microsoft.AspNetCore.Builder;

namespace FollowUP.Api.Framework
{
    public static class Extensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware(typeof(ExceptionHandlerMiddleware));
        public static IApplicationBuilder UseCustomTokenManager(this IApplicationBuilder builder)
            => builder.UseMiddleware(typeof(TokenManagerMiddleware));
    }
}