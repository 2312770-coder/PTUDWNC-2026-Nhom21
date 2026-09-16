using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Queries.GetCurrentUser;

// FR-AUTH-006 - CHƯA HIỆN THỰC.
//
// Gợi ý: lấy _currentUser.UserId (đọc từ JWT claims, không cần query DB để biết
// là ai), sau đó FindByIdAsync + GetRolesAsync để lấy đủ thông tin trả về UserDto.
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-006 (Xem hồ sơ cá nhân) chưa được hiện thực.");
}
