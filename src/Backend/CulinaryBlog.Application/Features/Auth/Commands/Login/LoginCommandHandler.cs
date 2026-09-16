using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.Login;

// FR-AUTH-002 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-AUTH-002.
//
// Gợi ý các bước:
//   1. Tìm user theo email. Không thấy -> UnauthorizedException với thông báo
//      CHUNG CHUNG ("Email hoặc mật khẩu không đúng"), không nói rõ sai cái nào
//      để tránh lộ email nào đang tồn tại trong hệ thống.
//   2. Kiểm tra user.IsActive - tài khoản bị Admin khóa thì không cho vào.
//   3. Kiểm tra IsLockedOutAsync (khóa tạm sau 5 lần sai).
//   4. CheckPasswordAsync; sai thì gọi AccessFailedAsync để tăng bộ đếm.
//   5. Đúng thì ResetAccessFailedCountAsync.
//   6. Sinh Access Token + Refresh Token qua IJwtService. LƯU Ý: chỉ lưu
//      TokenHash vào DB, trả RawToken về cho client (SRS mục 7.8).
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokensDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager,
        IJwtService jwtService, IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }

    public Task<AuthTokensDto> Handle(LoginCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-002 (Đăng nhập email/mật khẩu) chưa được hiện thực.");
}
