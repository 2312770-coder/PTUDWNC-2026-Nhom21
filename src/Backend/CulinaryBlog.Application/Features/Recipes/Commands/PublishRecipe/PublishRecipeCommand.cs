using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.PublishRecipe;

// FR-RCP-005: Xuất bản / hủy xuất bản. SRS mục 8.3:
//   PATCH /recipes/{id}/publish    (Draft -> Published)
//   PATCH /recipes/{id}/unpublish  (Published -> Draft)
// Dùng chung 1 Command, phân biệt bằng cờ Publish.
public record PublishRecipeCommand(Guid Id, bool Publish) : IRequest;
