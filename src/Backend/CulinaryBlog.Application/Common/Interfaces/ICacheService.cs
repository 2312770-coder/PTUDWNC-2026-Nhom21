namespace CulinaryBlog.Application.Common.Interfaces;

// Cache phân tán qua Redis. NFR-SCALE yêu cầu không dùng IMemoryCache cho
// state dùng chung, vì backend phải scale-out nhiều instance
// (xem docs/DECISIONS.md D4).
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
}
