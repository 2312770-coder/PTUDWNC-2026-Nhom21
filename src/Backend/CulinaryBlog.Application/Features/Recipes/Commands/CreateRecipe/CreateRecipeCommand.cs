using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

// ============================================================================
// DTOs hỗ trợ tạo nguyên liệu và các bước thực hiện đi kèm khi tạo công thức mới
// theo đúng đặc tả SRS mục 3.3 (FR-RCP-003) và mục 8.3 (POST /api/v1/recipes).
// ============================================================================

/// <summary>
/// Thông tin nguyên liệu đi kèm khi tạo công thức.
/// Quantity và Unit là nullable theo SRS mục 7.4 (hỗ trợ gia vị nêm nếm vừa đủ).
/// </summary>
public record CreateIngredientDto(
    string Name,
    decimal? Quantity = null,
    string? Unit = null,
    string? Notes = null,
    int OrderIndex = 0
);

/// <summary>
/// Thông tin bước thực hiện đi kèm khi tạo công thức.
/// StepNumber có thể null để server tự động đánh số thứ tự tăng dần liên tục.
/// TimerMinutes dùng cho tính năng hẹn giờ (SRS mục 7.3).
/// </summary>
public record CreateStepDto(
    string Title,
    string Description,
    int? StepNumber = null,
    int? TimerMinutes = null,
    string? ImageUrl = null
);

// FR-RCP-003: Tạo công thức mới [Author/Admin]. SRS mục 8.3: POST /recipes
// Recipe mới tạo luôn ở trạng thái Draft (bản nháp), sau đó phải qua quy trình kiểm tra
// điều kiện (ít nhất 1 nguyên liệu và 1 bước) rồi mới Publish riêng (FR-RCP-005).
public record CreateRecipeCommand(
    string Title,
    string Description,
    string Instructions,
    Guid CategoryId,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    NutritionDto? Nutrition = null,
    IReadOnlyList<CreateIngredientDto>? Ingredients = null,
    IReadOnlyList<CreateStepDto>? Steps = null
) : IRequest<RecipeDetailDto>;

