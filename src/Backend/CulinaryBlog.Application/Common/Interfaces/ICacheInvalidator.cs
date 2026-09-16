namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 6.3 - Command nào implement interface này thì sau khi chạy thành công,
// CacheInvalidationBehavior sẽ tự xóa các cache key liên quan.
public interface ICacheInvalidator
{
    // Các prefix cache cần xóa, vd: "categories:" khi tạo/sửa/xóa danh mục.
    IEnumerable<string> CacheKeyPrefixes { get; }
}
