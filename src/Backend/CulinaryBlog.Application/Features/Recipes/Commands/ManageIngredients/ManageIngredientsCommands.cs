using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageIngredients;

// FR-RCP-009: Quản lý nguyên liệu (CRUD RecipeIngredient). SRS mục 8.6:
//   POST   /recipes/{id}/ingredients
//   PUT    /recipes/{id}/ingredients/{ingId}
//   DELETE /recipes/{id}/ingredients/{ingId}

public record AddIngredientCommand(
    Guid RecipeId,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int? OrderIndex
) : IRequest<RecipeIngredientDto>;

public record UpdateIngredientCommand(
    Guid RecipeId,
    Guid IngredientId,
    string? Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int? OrderIndex
) : IRequest<RecipeIngredientDto>;

public record DeleteIngredientCommand(Guid RecipeId, Guid IngredientId) : IRequest;
