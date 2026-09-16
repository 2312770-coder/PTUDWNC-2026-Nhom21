using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

// FR-RCP-003: Tạo công thức mới [Author/Admin]. SRS mục 8.3: POST /recipes
// Recipe mới luôn ở trạng thái Draft, phải publish riêng (FR-RCP-005).
public record CreateRecipeCommand(
    string Title,
    string Description,
    string Instructions,
    Guid CategoryId,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    NutritionDto? Nutrition
) : IRequest<RecipeDetailDto>;
