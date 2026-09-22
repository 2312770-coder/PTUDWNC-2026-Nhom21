using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using CulinaryBlog.Domain.Enums; // Thêm dòng này để dùng RecipeStatus
using MediatR;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để dùng ToListAsync()

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

// FR-CAT-001 - Xem danh sách danh mục (kèm số công thức Published và sắp xếp theo OrderIndex).
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

   public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await _categoryRepository.Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.OrderIndex)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug, 
                c.Description,
                c.ImageUrl,
                c.OrderIndex,
                c.Recipes.Count(r => r.Status == RecipeStatus.Published && !r.IsDeleted)
            ))
            .ToListAsync(ct);

        return categories;
    }
}