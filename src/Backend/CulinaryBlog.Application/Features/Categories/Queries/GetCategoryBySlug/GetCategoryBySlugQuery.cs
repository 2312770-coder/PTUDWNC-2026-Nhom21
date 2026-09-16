using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;

// FR-CAT-002: Xem chi tiết danh mục + danh sách công thức thuộc danh mục.
// SRS mục 8.2: GET /categories/{slug}?page=1&pageSize=10&sortBy=...
// -> 200 { category, recipes: PagedResult }
public record GetCategoryBySlugQuery(string Slug, PagingParams Paging)
    : IRequest<CategoryWithRecipesDto>;

// Kiểu trả về gộp cả thông tin danh mục lẫn danh sách công thức có phân trang.
public record CategoryWithRecipesDto(
    CategoryDto Category,
    PagedResult<RecipeListItemDto> Recipes
);
