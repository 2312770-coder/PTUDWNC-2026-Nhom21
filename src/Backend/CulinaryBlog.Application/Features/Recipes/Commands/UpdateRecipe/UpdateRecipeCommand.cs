using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;

// FR-RCP-004: Cập nhật công thức [Author-Owner/Admin]. SRS mục 8.3: PUT /recipes/{id}
// Tất cả field optional (partial update).
public record UpdateRecipeCommand(
    Guid Id,
    string? Title,
    string? Description,
    string? Instructions,
    Guid? CategoryId,
    int? PrepTime,
    int? CookTime,
    int? Servings,
    RecipeDifficulty? Difficulty,
    NutritionDto? Nutrition
) : IRequest<RecipeDetailDto>;
