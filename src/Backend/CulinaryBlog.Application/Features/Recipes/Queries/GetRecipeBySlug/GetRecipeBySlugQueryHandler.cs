using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

// FR-RCP-002: Xem chi tiết công thức kèm đầy đủ các bước, nguyên liệu, ảnh và thông tin dinh dưỡng.
public class GetRecipeBySlugQueryHandler : IRequestHandler<GetRecipeBySlugQuery, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public GetRecipeBySlugQueryHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public async Task<RecipeDetailDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
    {
        var recipe = await _recipeRepository.GetDetailBySlugAsync(request.Slug, ct);
        if (recipe == null)
        {
            throw new NotFoundException($"Không tìm thấy công thức với đường dẫn '{request.Slug}'.");
        }

        if (recipe.Status != RecipeStatus.Published)
        {
            var canView = _currentUser.UserId == recipe.AuthorId || _currentUser.IsAdmin;
            if (!canView)
            {
                throw new NotFoundException($"Không tìm thấy công thức với đường dẫn '{request.Slug}'.");
            }
        }

        return new RecipeDetailDto(
            recipe.Id,
            recipe.Title,
            recipe.Slug,
            recipe.Description,
            recipe.Instructions,
            recipe.PrepTime,
            recipe.CookTime,
            recipe.Servings,
            recipe.Difficulty.ToString(),
            recipe.Status.ToString(),
            recipe.PublishedAt,
            new CategoryRefDto(recipe.Category!.Id, recipe.Category.Name, recipe.Category.Slug),
            new AuthorRefDto(recipe.Author!.Id, recipe.Author.DisplayName, recipe.Author.AvatarUrl),
            recipe.Nutrition != null ? new NutritionDto(
                recipe.Nutrition.Calories,
                recipe.Nutrition.Protein,
                recipe.Nutrition.Carbohydrates,
                recipe.Nutrition.Fat,
                recipe.Nutrition.Fiber,
                recipe.Nutrition.Sodium
            ) : null,
            recipe.Steps.OrderBy(s => s.StepNumber).Select(s => new RecipeStepDto(
                s.Id,
                s.StepNumber,
                s.Title,
                s.Description,
                s.TimerMinutes,
                s.ImageUrl
            )).ToList(),
            recipe.Ingredients.OrderBy(i => i.OrderIndex).Select(i => new RecipeIngredientDto(
                i.Id,
                i.Name,
                i.Quantity,
                i.Unit,
                i.Notes,
                i.OrderIndex
            )).ToList(),
            recipe.Images.OrderBy(i => i.OrderIndex).Select(i => new RecipeImageDto(
                i.Id,
                i.OriginalUrl,
                i.MediumUrl,
                i.ThumbnailUrl,
                i.AltText,
                i.IsPrimary,
                i.OrderIndex
            )).ToList()
        );
    }
}

