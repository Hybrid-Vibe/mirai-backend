using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Filters
{
    public class ValidateTurnstileAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var token = context.HttpContext
                .Request
                .Headers["X-Turnstile-Token"]
                .FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new BadRequestObjectResult(
                    new
                    {
                        message = "Missing Turnstile token"
                    });

                return;
            }

            var service = context.HttpContext
                .RequestServices
                .GetRequiredService<ITurnstileService>();

            var valid = await service.VerifyAsync(token);

            if (!valid)
            {
                context.Result = new BadRequestObjectResult(
                    new
                    {
                        message = "Invalid Turnstile"
                    });

                return;
            }

            await next();
        }
    }
}
