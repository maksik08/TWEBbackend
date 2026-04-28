using System.Text.Json;
using ProjectBackend.api.Exceptions;

namespace ProjectBackend.api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;

                var payload = JsonSerializer.Serialize(new { message = ex.Message });
                await context.Response.WriteAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled server error.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var payload = JsonSerializer.Serialize(new { message = "An unexpected server error occurred." });
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
