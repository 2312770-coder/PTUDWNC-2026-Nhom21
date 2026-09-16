using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Application.Common.Behaviors;

// SRS mục 6.3 - đo thời gian xử lý, cảnh báo nếu vượt 500ms
// (khớp ngưỡng p95 <= 500ms trong NFR-PERF).
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int WarningThresholdMs = 500;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();

        if (timer.ElapsedMilliseconds > WarningThresholdMs)
        {
            _logger.LogWarning(
                "Request chậm: {RequestName} mất {ElapsedMs}ms (ngưỡng {Threshold}ms)",
                typeof(TRequest).Name, timer.ElapsedMilliseconds, WarningThresholdMs);
        }

        return response;
    }
}
