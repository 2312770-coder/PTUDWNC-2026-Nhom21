using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;

/// <summary>
/// FR-CAT-002: Query lấy thông tin chi tiết danh mục kèm danh sách công thức thuộc danh mục đó.
/// </summary>
public record GetCategoryBySlugQuery(string Slug) : IRequest<CategoryDetailDto>;
