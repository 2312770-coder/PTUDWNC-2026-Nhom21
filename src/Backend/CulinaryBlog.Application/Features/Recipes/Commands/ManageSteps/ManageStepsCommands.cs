using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageSteps;

// FR-RCP-010: Quản lý các bước thực hiện (CRUD RecipeStep). SRS mục 8.5:
//   POST   /recipes/{id}/steps
//   PUT    /recipes/{id}/steps/{stepId}
//   DELETE /recipes/{id}/steps/{stepId}

public record AddStepCommand(
    Guid RecipeId,
    string Title,
    string Description,
    int? StepNumber = null,
    int? TimerMinutes = null,
    string? ImageUrl = null
) : IRequest<RecipeStepDto>;

public record UpdateStepCommand(
    Guid RecipeId,
    Guid StepId,
    int? StepNumber,
    string? Title,
    string? Description,
    int? TimerMinutes,
    string? ImageUrl
) : IRequest<RecipeStepDto>;

public record DeleteStepCommand(Guid RecipeId, Guid StepId) : IRequest;
