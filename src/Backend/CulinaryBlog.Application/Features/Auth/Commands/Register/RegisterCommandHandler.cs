using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.Register;

// FR-AUTH-001 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-AUTH-001.
//
// Gợi ý các bước (đọc kỹ FR-AUTH-001 trong SRS trước khi code):
//   1. Kiểm tra email đã tồn tại chưa -> nếu có, ném ConflictException (409).
//   2. Tạo user qua ApplicationUser.Create(email, displayName).
//   3. _userManager.CreateAsync(user, password) - Identity tự hash bằng PBKDF2
//      (CONS-004), tuyệt đối không tự viết code hash.
//   4. Nếu thất bại: gom result.Errors -> ném ValidationException (400).
//   5. Gán role mặc định "Author" (_userManager.AddToRoleAsync).
//   6. Đẩy job gửi email chào mừng (FR-JOB-001) - phối hợp với người làm FR-JOB.
//   7. Trả RegisterResponseDto (SRS mục 8.1 trả 201 với userId/email/displayName,
//      KHÔNG trả token ở bước này).
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-001 (Đăng ký tài khoản) chưa được hiện thực.");
}
