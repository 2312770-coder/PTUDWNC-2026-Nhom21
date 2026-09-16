using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;

// FR-AUTH-003 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-AUTH-003.
//
// Gợi ý các bước:
//   1. Xác minh IdToken với Google (thư viện Google.Apis.Auth:
//      GoogleJsonWebSignature.ValidateAsync) - KHÔNG tin token từ client
//      mà chưa verify. Sai -> ném ValidationException (400).
//   2. Lấy email, name, picture từ payload đã verify.
//   3. Tìm user theo email. Chưa có thì tạo bằng
//      ApplicationUser.CreateFromGoogle(...) và gán role "Author".
//      Đã có (đăng ký thủ công trước đó) thì liên kết external login vào
//      tài khoản đang có, không tạo trùng.
//   4. Sinh cặp token giống luồng đăng nhập thường.
public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthTokensDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public GoogleLoginCommandHandler(UserManager<ApplicationUser> userManager,
        IJwtService jwtService, IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }

    public Task<AuthTokensDto> Handle(GoogleLoginCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-003 (Đăng nhập Google OAuth) chưa được hiện thực.");
}
