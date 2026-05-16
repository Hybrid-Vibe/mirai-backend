using Mirai.Application.Interfaces.Services;

namespace Mirai.MiddleWare
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IJwtTokenGenerator blacklistService)
        {
            var authHeader = context.Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Replace("Bearer ", "");

                var isBlacklisted =
                    await blacklistService.IsBlacklistedAsync(token);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsync("Token is invalid");

                    return;
                }
            }

            await _next(context);
        }
    }
}
