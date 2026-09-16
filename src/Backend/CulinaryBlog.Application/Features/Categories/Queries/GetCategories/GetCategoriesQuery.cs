using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

// FR-CAT-001: Xem danh sách danh mục. SRS mục 8.2: GET /categories (không cần auth)
// -> 200 [{ id, name, slug, description, imageUrl, recipeCount }]
//
// Query này implement ICacheable nên CachingBehavior sẽ tự cache kết quả vào
// Redis, Handler không phải viết code cache. Danh mục ít thay đổi nên TTL dài.
public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>, ICacheable
{
    public string CacheKey => "categories:all";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
}
