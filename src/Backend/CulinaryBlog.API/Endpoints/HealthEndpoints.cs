using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CulinaryBlog.API.Endpoints;

// FR-OBS-001 / SRS mục 8.7 - 3 endpoint health check.
public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        // Tổng hợp mọi dependency (DB, Redis, MinIO).
        app.MapHealthChecks("/health").AllowAnonymous();

        // Liveness: chỉ cần process còn sống, không kiểm tra dependency nào.
        // Dùng cho Kubernetes/Docker restart container khi app treo.
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        }).AllowAnonymous();

        // Readiness: kiểm tra DB và Redis đã sẵn sàng nhận traffic chưa.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        }).AllowAnonymous();

        return app;
    }
}
