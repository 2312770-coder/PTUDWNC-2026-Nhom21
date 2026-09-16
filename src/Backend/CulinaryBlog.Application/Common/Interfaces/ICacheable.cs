namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 6.3 - Query nào implement interface này sẽ được CachingBehavior
// tự động kiểm tra Redis trước khi chạy Handler.
public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}
