using System.Text.Json;
using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CulinaryBlog.Infrastructure.Caching;

// Cache phân tán qua Redis. CachingBehavior và CacheInvalidationBehavior
// gọi service này, nên các Handler không cần biết gì về Redis.
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    // Lưu lại danh sách key đã tạo theo từng prefix, để hỗ trợ xóa hàng loạt.
    // IDistributedCache không có lệnh "xóa theo prefix" sẵn.
    private static readonly Dictionary<string, HashSet<string>> KeyIndex = new();
    private static readonly SemaphoreSlim IndexLock = new(1, 1);

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        await _cache.SetAsync(key, bytes,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);

        await TrackKeyAsync(key);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => _cache.RemoveAsync(key, ct);

    // Xóa mọi key bắt đầu bằng prefix (vd "categories:" khi danh mục thay đổi).
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        await IndexLock.WaitAsync(ct);
        try
        {
            if (!KeyIndex.TryGetValue(prefix, out var keys)) return;

            foreach (var key in keys.ToList())
                await _cache.RemoveAsync(key, ct);

            keys.Clear();
        }
        finally
        {
            IndexLock.Release();
        }
    }

    private async Task TrackKeyAsync(string key)
    {
        var separatorIndex = key.IndexOf(char.Parse(":"));
        if (separatorIndex < 0) return;

        var prefix = key[..(separatorIndex + 1)];

        await IndexLock.WaitAsync();
        try
        {
            if (!KeyIndex.TryGetValue(prefix, out var keys))
            {
                keys = new HashSet<string>();
                KeyIndex[prefix] = keys;
            }
            keys.Add(key);
        }
        finally
        {
            IndexLock.Release();
        }
    }
}
