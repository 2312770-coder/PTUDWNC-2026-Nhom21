using System.Security.Claims;
using CulinaryBlog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CulinaryBlog.Infrastructure.Authorization;

// SRS mục 2.3 - tầng phân quyền thứ 2 (Resource-Based Authorization):
// Author chỉ sửa/xóa được recipe của CHÍNH MÌNH (AuthorId == currentUserId),
// Admin thì toàn quyền. Đây là điểm giao giữa module FR-AUTH và FR-RCP.
public class RecipeAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, Recipe>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Recipe recipe)
    {
        // Admin có toàn quyền trên mọi recipe.
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Đọc recipe đã publish thì ai cũng được.
        if (requirement.Name == RecipeOperations.Read.Name &&
            recipe.Status == Domain.Enums.RecipeStatus.Published)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Task.CompletedTask;

        // Chủ sở hữu được đọc/sửa/xóa recipe của mình (kể cả bản nháp).
        if (userId == recipe.AuthorId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
