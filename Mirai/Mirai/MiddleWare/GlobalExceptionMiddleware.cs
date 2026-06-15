using System.Text.Json;
using Mirai.Exceptions;

namespace Mirai.MiddleWare
{
    public class GlobalExceptionMiddleware
    {
    
            private readonly RequestDelegate _next;

            public GlobalExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            }

            private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                ErrorCode = "INTERNAL_ERROR",
                Message = exception.ToString()
                    
                };

                switch (exception)
                {
                case UserFriendlyException userEx:
                    context.Response.StatusCode = 400;
                    response = new
                    {
                        Success = false,
                        ErrorCode = userEx.ErrorCode.ToString(),
                        Message = userEx.Message
                    };
                    break;

                case Mirai.Application.Exceptions.UserFriendlyException appEx:
                    context.Response.StatusCode = appEx.StatusCode;
                    response = new
                    {
                        Success = false,
                        ErrorCode = appEx.ErrorCode,
                        Message = appEx.Message
                    };
                    break;

                default:
                    context.Response.StatusCode = 500;
                    Console.WriteLine($"Unhandled exception: {exception}");
                    break;

            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
