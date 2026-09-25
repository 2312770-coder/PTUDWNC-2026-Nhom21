using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

// FR-CAT-003: Admin tạo danh mục mới.
// Tự động xóa cache "categories:" sau khi tạo thành công qua ICacheInvalidator.
public record CreateCategoryCommand(
    string Name,
    string? Description = null,
    string? ImageUrl = null,
    int OrderIndex = 0
) : IRequest<CategoryDto>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeyPrefixes => new[] { "categories:" };
}
