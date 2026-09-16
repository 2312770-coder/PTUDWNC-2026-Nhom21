using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

// FR-CAT-003: Tạo danh mục mới [Admin]. SRS mục 8.2: POST /categories
// Body: { name, description?, imageUrl? } -> 201 { id, name, slug, description }
//
// Implement ICacheInvalidator để sau khi tạo xong, CacheInvalidationBehavior
// tự xóa cache danh sách danh mục (nếu không thì client vẫn thấy dữ liệu cũ).
public record CreateCategoryCommand(string Name, string? Description, string? ImageUrl)
    : IRequest<CategoryDto>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeyPrefixes => new[] { "categories:" };
}
