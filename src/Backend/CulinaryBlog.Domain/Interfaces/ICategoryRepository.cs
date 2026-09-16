using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Domain.Interfaces;

// SRS mục 6.2 - repository riêng cho Category.
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);

    // Dùng cho FR-CAT-005: không cho xóa danh mục còn công thức bên trong.
    Task<bool> HasRecipesAsync(Guid categoryId, CancellationToken ct = default);
}
