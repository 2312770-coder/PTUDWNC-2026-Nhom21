using CulinaryBlog.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Application.Common.Behaviors;

// SRS mục 6.3 - Pipeline thứ 5. Sau khi Command chạy thành công, tự xóa các
// cache key liên quan để lần đọc sau lấy dữ liệu mới.
// Cách dùng: cho Command implement thêm ICacheInvalidator.
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICacheService _cache;
    private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(ICacheService cache,
        ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(); // chạy Handler trước, thành công mới xóa cache

        if (request is ICacheInvalidator invalidator)
        {
            foreach (var prefix in invalidator.CacheKeyPrefixes)
            {
                await _cache.RemoveByPrefixAsync(prefix, ct);
                _logger.LogInformation("Đã xóa cache theo prefix: {Prefix}", prefix);
            }
        }

        return response;
    }
}
