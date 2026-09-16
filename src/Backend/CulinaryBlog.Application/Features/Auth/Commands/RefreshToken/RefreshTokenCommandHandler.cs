using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.RefreshToken;

// FR-AUTH-004 - CHƯA HIỆN THỰC. Đây là phần khó nhất của module Auth,
// đọc kỹ FR-AUTH-004 và mục 7.8 trong SRS trước khi code.
//
// Gợi ý các bước (Refresh Token Rotation):
//   1. Băm raw token nhận được: _jwtService.HashRefreshToken(request.RefreshToken).
//      DB chỉ lưu hash, nên phải băm rồi mới tra cứu.
//   2. Tìm RefreshToken theo TokenHash. Không thấy -> UnauthorizedException.
//   3. Nếu token đã bị revoke NHƯNG có ReplacedByTokenHash -> dấu hiệu token
//      bị đánh cắp và dùng lại. Xử lý: thu hồi TOÀN BỘ token còn hiệu lực của
//      user đó (revoke cả token family), buộc đăng nhập lại.
//   4. Nếu token hết hạn hoặc đã revoke -> UnauthorizedException (401).
//   5. Hợp lệ: sinh cặp token mới, gọi oldToken.Revoke(newTokenHash) để
//      đánh dấu đã bị thay thế, lưu token mới vào DB.
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager,
        IJwtService jwtService, IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }

    public Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-004 (Làm mới Access Token / Token Rotation) chưa được hiện thực.");
}
