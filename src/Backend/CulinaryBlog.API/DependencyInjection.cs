using System.Globalization;
using System.Threading.RateLimiting;
using CulinaryBlog.API.Middleware;
using Microsoft.AspNetCore.RateLimiting;

namespace CulinaryBlog.API;

// SRS mục 6.2 - extension method AddPresentation() gom cấu hình của tầng
// Presentation (OpenAPI, exception handler, rate limiting, CORS, health check).
public static class DependencyInjection
{
    private const int LoginPermitLimit = 5;

    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();

        // CONS-005: mọi lỗi trả về theo RFC 7807.
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
        services.AddProblemDetails();

        // ── Rate Limiting ────────────────────────────────────────────────────
        services.AddRateLimiter(options =>
        {
            // Endpoint đăng nhập: 5 request/phút để chống dò mật khẩu.
            options.AddFixedWindowLimiter("login-policy", o =>
            {
                o.Window = TimeSpan.FromMinutes(1);
                o.PermitLimit = LoginPermitLimit;
                o.QueueLimit = 0;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // SRS mục 5.2 yêu cầu trả kèm các header X-RateLimit-* và Retry-After.
            options.OnRejected = async (context, ct) =>
            {
                var retryAfterSeconds = 60;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    retryAfterSeconds = (int)retryAfter.TotalSeconds;

                var headers = context.HttpContext.Response.Headers;
                headers["Retry-After"] = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
                headers["X-RateLimit-Limit"] = LoginPermitLimit.ToString(CultureInfo.InvariantCulture);
                headers["X-RateLimit-Remaining"] = "0";
                headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow
                    .AddSeconds(retryAfterSeconds).ToUnixTimeSeconds()
                    .ToString(CultureInfo.InvariantCulture);

                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "about:blank",
                    title = "Quá nhiều yêu cầu.",
                    status = StatusCodes.Status429TooManyRequests,
                    detail = $"Bạn đã gửi quá nhiều yêu cầu. Vui lòng thử lại sau {retryAfterSeconds} giây.",
                }, ct);
            };
        });

        // ── CORS (SRS mục 5.2) ───────────────────────────────────────────────
        services.AddCors(options =>
        {
            options.AddPolicy("NextJsPolicy", policy =>
            {
                var origins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:3000" };

                policy.WithOrigins(origins)
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .WithHeaders("Authorization", "Content-Type", "X-Correlation-ID")
                    .AllowCredentials();
            });
        });

        return services;
    }
}
