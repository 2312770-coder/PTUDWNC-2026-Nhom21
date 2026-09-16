namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 3.5 (FR-FILE) - abstraction cho object storage, cho phép đổi
// implementation (MinIO <-> AWS S3 <-> local filesystem) mà không sửa
// tầng Application.
// CONS-007: tối đa 5MB, chỉ nhận image/jpeg, image/png, image/webp, image/avif,
// phải kiểm tra magic bytes chứ không chỉ nhìn phần mở rộng file.
public interface IFileStorageService
{
    // Trả về public URL của file sau khi upload.
    // Tên file sinh dạng {folder}/{Guid.NewGuid()}{ext} để chống path traversal.
    Task<string> UploadAsync(Stream content, string fileName, string contentType,
        string folder, CancellationToken ct = default);

    // Idempotent: object không tồn tại thì bỏ qua, không ném exception.
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
}
