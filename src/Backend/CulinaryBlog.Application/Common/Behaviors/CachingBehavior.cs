using CulinaryBlog.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Application.Common.Behaviors;

// SRS mục 6.3 - Pipeline thứ 3. Chỉ áp dụng cho Query có implement ICacheable.
// Cách dùng: cho Query kế thừa thêm ICacheable và khai báo CacheKey + CacheDuration,
// phần cache sẽ tự động chạy, Handler không cần biết gì về Redis.
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICacheable cacheable) return await next();

        var cached = await _cache.GetAsync<TResponse>(cacheable.CacheKey, ct);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT: {CacheKey}", cacheable.CacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS: {CacheKey}", cacheable.CacheKey);
        var response = await next();

        if (response is not null)
            await _cache.SetAsync(cacheable.CacheKey, response, cacheable.CacheDuration, ct);

        return response;
    }
}
