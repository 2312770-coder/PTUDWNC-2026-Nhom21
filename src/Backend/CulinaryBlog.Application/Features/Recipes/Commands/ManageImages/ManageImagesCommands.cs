using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageImages;

// FR-RCP-008: Quản lý ảnh công thức (upload / set primary / delete).
// SRS mục 8.4:
//   POST   /recipes/{id}/images                -> upload ảnh mới
//   PATCH  /recipes/{id}/images/{imageId}      -> sửa altText/isPrimary/orderIndex
//   DELETE /recipes/{id}/images/{imageId}      -> xóa ảnh
//
// CONS-007: tối đa 5MB, chỉ nhận jpeg/png/webp/avif, phải kiểm tra magic bytes
// chứ không chỉ nhìn đuôi file.

public record UploadRecipeImageCommand(
    Guid RecipeId,
    Stream Content,
    string FileName,
    string ContentType,
    long FileSize,
    string? AltText,
    bool IsPrimary
) : IRequest<RecipeImageDto>;

public record UpdateRecipeImageCommand(
    Guid RecipeId,
    Guid ImageId,
    string? AltText,
    bool? IsPrimary,
    int? OrderIndex
) : IRequest<RecipeImageDto>;

public record DeleteRecipeImageCommand(Guid RecipeId, Guid ImageId) : IRequest;
