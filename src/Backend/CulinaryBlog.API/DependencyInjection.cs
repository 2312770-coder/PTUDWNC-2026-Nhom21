using CulinaryBlog.API.Middleware;

namespace CulinaryBlog.API;

// SRS mục 6.2 - extension method AddPresentation() gom cấu hình của tầng
// Presentation (OpenAPI, exception handler, CORS).
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();

        // CONS-005: mọi lỗi trả về theo RFC 7807.
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
        services.AddProblemDetails();

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
