using System.Text.Json;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Middleware
{
    public class IdempotencyMiddleware
    {
        private const string HeaderName = "Idempotency-Key";
        private const string ReplayedHeaderName = "Idempotent-Replayed";
        private const int MaxKeyLength = 100;
        private static readonly TimeSpan RecordLifetime = TimeSpan.FromHours(24);

        private readonly RequestDelegate _next;
        private readonly ILogger<IdempotencyMiddleware> _logger;

        public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IIdempotencyStore store, ICurrentUserContext currentUser)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var keyValues))
            {
                await _next(context);
                return;
            }

            var key = keyValues.ToString();
            if (string.IsNullOrWhiteSpace(key) || key.Length > MaxKeyLength)
            {
                await WriteJsonAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    ApiResponse<object?>.Fail($"Header '{HeaderName}' must be 1..{MaxKeyLength} characters."));
                return;
            }

            var userId = currentUser.UserId;
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            var cancellationToken = context.RequestAborted;

            var existing = await store.GetAsync(userId, key, method, path, cancellationToken);
            if (existing != null)
            {
                _logger.LogInformation(
                    "Idempotent replay for key={Key} user={UserId} {Method} {Path}",
                    key, userId, method, path);

                context.Response.StatusCode = existing.StatusCode;
                context.Response.ContentType = existing.ContentType;
                context.Response.Headers[ReplayedHeaderName] = "true";
                await context.Response.WriteAsync(existing.ResponseBody, cancellationToken);
                return;
            }

            var originalBody = context.Response.Body;
            using var capture = new MemoryStream();
            context.Response.Body = capture;

            try
            {
                await _next(context);

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    capture.Position = 0;
                    using var reader = new StreamReader(capture, leaveOpen: true);
                    var body = await reader.ReadToEndAsync(cancellationToken);

                    var saved = await store.TrySaveAsync(new IdempotencyRecordDomain
                    {
                        Key = key,
                        UserId = userId,
                        Method = method,
                        Path = path,
                        StatusCode = context.Response.StatusCode,
                        ContentType = context.Response.ContentType ?? "application/json",
                        ResponseBody = body,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.Add(RecordLifetime)
                    }, cancellationToken);

                    if (!saved)
                    {
                        _logger.LogInformation(
                            "Idempotency record collision for key={Key} user={UserId} (concurrent request)",
                            key, userId);
                    }
                }

                capture.Position = 0;
                await capture.CopyToAsync(originalBody, cancellationToken);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private static Task WriteJsonAsync(HttpContext context, int statusCode, object payload)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
