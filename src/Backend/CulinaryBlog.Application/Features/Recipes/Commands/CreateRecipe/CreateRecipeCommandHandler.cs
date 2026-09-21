using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>
/// Handler xử lý Command tạo công thức mới (FR-RCP-003, SRS mục 3.3 và 8.3).
/// TODO: Người phụ trách FR-RCP-003 hiện thực handler này.
/// </summary>
public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;

    public CreateRecipeCommandHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public Task<RecipeDetailDto> Handle(CreateRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException("FR-RCP-003 (Tạo công thức mới) chưa được hiện thực.");
}
