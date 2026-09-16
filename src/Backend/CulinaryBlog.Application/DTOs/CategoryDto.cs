namespace CulinaryBlog.Application.DTOs;

// SRS mục 8.2 - GET /categories
public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int OrderIndex,
    int RecipeCount
);
