using Microsoft.AspNetCore.Authorization;

namespace CulinaryBlog.Infrastructure.Authorization;

// SRS mục 2.3 - tầng phân quyền thứ 3 (Policy-Based Authorization):
// policy "VerifiedAuthor" yêu cầu email đã được xác nhận.
public class VerifiedAuthorRequirement : IAuthorizationRequirement { }

public class VerifiedAuthorHandler : AuthorizationHandler<VerifiedAuthorRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, VerifiedAuthorRequirement requirement)
    {
        // Admin luôn qua.
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // TODO (người phụ trách FR-AUTH): khi hiện thực JwtService, nhớ thêm
        // claim "email_verified" vào Access Token thì đoạn kiểm tra này mới
        // có tác dụng thật.
        var emailVerified = context.User.FindFirst("email_verified")?.Value;
        if (string.Equals(emailVerified, "true", StringComparison.OrdinalIgnoreCase))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
