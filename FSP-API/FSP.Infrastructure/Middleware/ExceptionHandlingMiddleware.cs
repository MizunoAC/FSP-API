using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace FSP.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpRequestException httpEx)
            {
                await HandleHttpRequestExceptionAsync(context, httpEx);
            }
            catch (Exception ex)
            {
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private static async Task HandleHttpRequestExceptionAsync(HttpContext context, HttpRequestException httpEx)
        {
            var statusCode = httpEx.StatusCode switch
            {
                HttpStatusCode.BadRequest => StatusCodes.Status400BadRequest,
                HttpStatusCode.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = statusCode,
                error = true,
                message = httpEx.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 500,
                error = "Unexpected error"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
