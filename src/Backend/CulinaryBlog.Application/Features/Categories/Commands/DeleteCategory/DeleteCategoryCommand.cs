using CulinaryBlog.Application.Common.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.DeleteCategory;

// FR-CAT-005: Xóa danh mục [Admin]. SRS mục 8.2: DELETE /categories/{id}
// -> 204. Trả 409 nếu danh mục vẫn còn công thức bên trong.
// Đây là SOFT DELETE (đặt IsDeleted = true), theo SRS mục 7.1 và 8.2.
public record DeleteCategoryCommand(Guid Id) : IRequest, ICacheInvalidator
{
    public IEnumerable<string> CacheKeyPrefixes => new[] { "categories:" };
}
