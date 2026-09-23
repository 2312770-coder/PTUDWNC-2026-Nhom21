using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;

/// <summary>
/// FR-CAT-002: Handler xử lý lấy chi tiết danh mục kèm danh sách công thức đã xuất bản (Published).
/// 
/// Nghiệp vụ:
/// 1. Tìm Category theo Slug (không phân biệt hoa thường, chưa bị xóa mềm).
/// 2. Nạp các Recipes thuộc danh mục có Status == Published và !IsDeleted, sắp xếp theo PublishedAt mới nhất.
/// 3. Nếu không tìm thấy, ném NotFoundException.
/// 4. Ánh xạ sang CategoryDetailDto kèm IReadOnlyList<RecipeListItemDto>.
/// </summary>
public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryDetailDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryBySlugQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDetailDto> Handle(GetCategoryBySlugQuery request, CancellationToken ct)
    {
        var slug = request.Slug?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new NotFoundException("Danh mục không hợp lệ.");
        }

        // Truy vấn category kèm danh sách công thức đã Published
        var category = await _categoryRepository.Query()
            .Where(c => !c.IsDeleted && c.Slug.ToLower() == slug)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.ImageUrl,
                c.OrderIndex,
                PublishedRecipeCount = c.Recipes.Count(r => !r.IsDeleted && r.Status == RecipeStatus.Published),
                Recipes = c.Recipes
                    .Where(r => !r.IsDeleted && r.Status == RecipeStatus.Published)
                    .OrderByDescending(r => r.PublishedAt ?? r.CreatedAt)
                    .Select(r => new RecipeListItemDto(
                        r.Id,
                        r.Title,
                        r.Slug,
                        r.Description,
                        r.PrepTime,
                        r.CookTime,
                        r.Servings,
                        r.Difficulty.ToString(),
                        r.Status.ToString(),
                        r.Images.Where(img => img.IsPrimary).Select(img => img.OriginalUrl).FirstOrDefault()
                            ?? r.Images.Select(img => img.OriginalUrl).FirstOrDefault(),
                        c.Name,
                        r.Author != null ? (r.Author.DisplayName ?? r.Author.UserName ?? "Ẩn danh") : "Ẩn danh",
                        r.PublishedAt
                    ))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (category == null)
        {
            throw new NotFoundException($"Không tìm thấy danh mục với slug '{request.Slug}'.");
        }

        return new CategoryDetailDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ImageUrl,
            category.OrderIndex,
            category.PublishedRecipeCount,
            category.Recipes
        );
    }
}
