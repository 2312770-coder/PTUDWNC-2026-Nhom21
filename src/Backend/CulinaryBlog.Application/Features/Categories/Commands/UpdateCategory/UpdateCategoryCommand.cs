using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.UpdateCategory;

// FR-CAT-004: Cập nhật danh mục [Admin]. SRS mục 8.2: PUT /categories/{id}
// Body: { name, description?, imageUrl?, orderIndex? }
public record UpdateCategoryCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? ImageUrl,
    int? OrderIndex
) : IRequest<CategoryDto>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeyPrefixes => new[] { "categories:" };
}
