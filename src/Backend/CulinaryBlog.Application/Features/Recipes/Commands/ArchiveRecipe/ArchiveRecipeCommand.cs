using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ArchiveRecipe;

// FR-RCP-006: Lưu trữ công thức. SRS mục 8.3: PATCH /recipes/{id}/archive
// Khác với xóa: dữ liệu vẫn còn, chỉ ẩn khỏi danh sách công khai.
public record ArchiveRecipeCommand(Guid Id) : IRequest;
