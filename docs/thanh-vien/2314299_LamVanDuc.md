# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Lâm Văn Đức (MSSV: 2314299)
### Vai trò: Authentication & Security

---

## 1. Thông Tin Chung
- **Họ và tên**: Lâm Văn Đức
- **MSSV**: 2314299
- **Email**: 2314299@dlu.edu.vn
- **GitHub**: [https://github.com/2314299-debug](https://github.com/2314299-debug)
- **Tổng số chức năng phụ trách**: **7 chức năng**

---

## 2. Danh Sách 7 Chức Năng Phụ Trách

| STT | Mã FR | Tên chức năng | File Backend cần làm | File Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- |
| 1 | **FR-AUTH-001** | Đăng ký tài khoản mới | `Features/Auth/Commands/Register/*` | `app/(auth)/register/page.tsx` |
| 2 | **FR-AUTH-002** | Đăng nhập Email/Mật khẩu + Rate Limiting | `Features/Auth/Commands/Login/*` | `app/(auth)/login/page.tsx` |
| 3 | **FR-AUTH-005** | Đăng xuất & Thu hồi token | `Features/Auth/Commands/Logout/*` | Nút Đăng xuất trên Navbar |
| 4 | **FR-AUTH-004** | Làm mới Access Token (Rotation) | `Features/Auth/Commands/RefreshToken/*` | `lib/api/client.ts` (Interceptor refresh) |
| 5 | **FR-AUTH-003** | Đăng nhập Google OAuth 2.0 | `Features/Auth/Commands/GoogleLogin/*` | Nút Google Sign-In trong form Login |
| 6 | **FR-RCP-005** | Xuất bản / Hủy xuất bản (D11) | `Features/Recipes/Commands/PublishRecipe/*` | Nút Xuất bản trong trang quản lý bài |
| 7 | **FR-JOB-001** | Gửi email chào mừng thành viên | `Infrastructure/Jobs/WelcomeEmailJob.cs` | Xem email gửi đến tại MailHog (:8025) |

---

## 3. Lộ Trình 6 Tuần (Tuần 2 → Tuần 7)

```
Tuần 2 (Đã hoàn thành):
  ✅ FR-AUTH-001 — Đăng ký tài khoản mới (Backend Identity + JWT & Frontend Form Register)

Tuần 3 (Tuần này):
  🔲 FR-AUTH-002: Đăng nhập Email/Mật khẩu + Tự cấu hình Rate Limiting chống Brute-Force
  🔲 FR-AUTH-005: Đăng xuất tài khoản (Thu hồi Refresh Token & Xóa Cookie/Session)

Tuần 4:
  🔲 FR-AUTH-004: Làm mới Access Token (Refresh Token Rotation)
  🔲 FR-AUTH-003: Đăng nhập Google OAuth 2.0

Tuần 5:
  🔲 FR-RCP-005: Xuất bản / Hủy xuất bản công thức (Tuân thủ điều kiện D11)
  🔲 FR-JOB-001: Hangfire background job gửi email chào mừng qua MailHog

Tuần 6:
  🔲 Tinh chỉnh bảo mật luồng Auth, Guard Route Next.js, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 3 (TUẦN NÀY LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2314299-LVDuc-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng 1: Đăng nhập Email/Mật khẩu + Rate Limiting (FR-AUTH-002)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2314299-LVDuc-Dang-Nhap
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Auth/Commands/Login/`:
   - `LoginCommand(string Email, string Password)` : `IRequest<AuthResponseDto>`
   - `LoginCommandValidator`: Email hợp lệ, Password không được để trống.
   - `LoginCommandHandler`:
     - Tìm user theo email: `await _userManager.FindByEmailAsync(request.Email)`. Nếu không có, ném `UnauthorizedException("Email hoặc mật khẩu không chính xác.")`.
     - Kiểm tra tài khoản có bị khóa không: `await _userManager.IsLockedOutAsync(user)`.
     - Kiểm tra mật khẩu: `await _userManager.CheckPasswordAsync(user, request.Password)`. Nếu sai, tăng số lần đăng nhập hỏng (`AccessFailedAsync`), nếu đủ 5 lần thì tự động khóa tạm thời 15 phút.
     - Nếu đúng, reset số lần sai (`ResetAccessFailedCountAsync`), sinh JWT Access Token + Refresh Token (lưu vào bảng `RefreshTokens`), trả về `AuthResponseDto`.
2. **Kỹ thuật Rate Limiting**:
   - Mở `src/Backend/CulinaryBlog.API/DependencyInjection.cs`, cấu hình Rate Limiting bằng ASP.NET Core `AddRateLimiter` (giới hạn 5 request/phút cho endpoint login).
   - Đăng ký route `POST /api/v1/auth/login` trong `AuthEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Tạo trang `src/Frontend/app/(auth)/login/page.tsx`:
   - Form nhập Email và Mật khẩu.
   - Xử lý gọi API POST `/api/v1/auth/login`.
   - Khi thành công: Lưu token vào Cookie/LocalStorage, cập nhật trạng thái User trên Navbar và chuyển hướng về trang chủ `/`.
   - Hiển thị thông báo lỗi rõ ràng nếu đăng nhập thất bại hoặc tài khoản bị khóa tạm thời.

---

### Chức năng 2: Đăng xuất & Thu hồi Token (FR-AUTH-005)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2314299-LVDuc-Dang-Xuat
```

#### Bước 2: Hiện thực Backend
1. Tạo `src/Backend/CulinaryBlog.Application/Features/Auth/Commands/Logout/`:
   - `LogoutCommand(string RefreshToken)`: `IRequest<bool>`
   - `LogoutCommandHandler`:
     - Tìm Refresh Token trong CSDL.
     - Đánh dấu thu hồi: `refreshToken.Revoke()` hoặc xóa khỏi bảng `RefreshTokens`.
     - Lưu thay đổi `await _context.SaveChangesAsync(ct)`.
2. Đăng ký endpoint POST `/api/v1/auth/logout` trong `AuthEndpoints.cs` (yêu cầu `RequireAuthorization`).

#### Bước 3: Hiện thực Frontend
1. Thêm nút "Đăng xuất" trong dropdown menu avatar trên `Navbar.tsx`.
2. Khi người dùng click Đăng xuất:
   - Gửi yêu cầu POST `/api/v1/auth/logout`.
   - Xóa token khỏi Storage/Cookie.
   - Chuyển hướng người dùng về trang đăng nhập `/login` hoặc reload trang chủ.
