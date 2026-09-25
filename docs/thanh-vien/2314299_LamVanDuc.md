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
| 3 | **FR-AUTH-005** | Đăng xuất & Thu hồi phiên làm việc | `Features/Auth/Commands/Logout/*` | Nút Đăng xuất trên Navbar |
| 4 | **FR-AUTH-004** | Làm mới Access Token (Rotation) | `Features/Auth/Commands/RefreshToken/*` | `lib/api/client.ts` (Interceptor refresh ngầm) |
| 5 | **FR-AUTH-003** | Đăng nhập Google OAuth 2.0 | `Features/Auth/Commands/GoogleLogin/*` | Nút Google Sign-In trong form Login |
| 6 | **FR-RCP-005** | Xuất bản / Hủy xuất bản (D11) | `Features/Recipes/Commands/PublishRecipe/*` | Nút Xuất bản trong trang quản lý bài |
| 7 | **FR-JOB-001** | Gửi email chào mừng thành viên mới | `Infrastructure/Jobs/WelcomeEmailJob.cs` | Xem email gửi đến tại MailHog (:8025) |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 8)

```
Tuần 2 (Đã hoàn thành & merge main):
  ✅ FR-AUTH-001 — Đăng ký tài khoản mới (Backend Identity + JWT & Frontend Form Register)

Tuần 3 (Đã hoàn thành & merge main):
  ✅ FR-AUTH-002 — Đăng nhập Email/Mật khẩu + Cấu hình Rate Limiting 5 req/phút chống Brute-Force

Tuần 4 (Tuần tới):
  🔲 FR-AUTH-005 — Đăng xuất tài khoản & Thu hồi Refresh Token trong CSDL (Backend + Frontend Navbar)
  🔲 FR-AUTH-004 — Làm mới Access Token (Refresh Token Rotation + Reuse Detection ngầm)

Tuần 5:
  🔲 FR-AUTH-003 — Đăng nhập bên thứ ba Google OAuth 2.0

Tuần 6:
  🔲 FR-RCP-005 — Xuất bản / Hủy xuất bản công thức (Tuân thủ điều kiện D11: >= 1 bước & >= 1 nguyên liệu)

Tuần 7:
  🔲 FR-JOB-001 — Hangfire background job tự động gửi email chào mừng qua MailHog

Tuần 8:
  🔲 Triển khai Production, Tinh chỉnh bảo mật luồng Auth, Guard Route Next.js, Hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2314299-LVDuc-<Ten-Chuc-Nang>`. **Không tự merge vào `main`** — báo trưởng nhóm Tiến để Tiến review và merge giúp.

---

### Chức năng: Đăng ký tài khoản (FR-AUTH-001)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2314299-LVDuc-Dang-Ky
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs`.
2. Thay thế dòng ném `NotImplementedException`:
   - Kiểm tra email đã tồn tại chưa: `await _userManager.FindByEmailAsync(request.Email)`. Nếu có ném `ConflictException("Email đã được sử dụng.")`.
   - Tạo user: `var user = ApplicationUser.Create(request.Email, request.DisplayName, request.UserName)`.
   - Lưu qua Identity: `var result = await _userManager.CreateAsync(user, request.Password)`.
   - Gán role "Author": `await _userManager.AddToRoleAsync(user, "Author")`.
   - Tạo Access Token và Refresh Token qua `_jwtTokenGenerator` và lưu refresh token vào database.
   - Trả về `AuthResponseDto`.
3. Kiểm tra validation trong `RegisterCommandValidator.cs` (Email hợp lệ, Mật khẩu >= 8 ký tự).

#### Bước 3: Hiện thực Frontend
1. Mở file `src/Frontend/app/(auth)/register/page.tsx`.
2. Dựng form đăng ký: Email, Tên hiển thị, Tên đăng nhập (tùy chọn), Mật khẩu, Xác nhận mật khẩu.
3. Khi submit, gọi `POST /api/v1/auth/register`, nếu thành công lưu token và chuyển hướng về trang chủ.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-001 dang ky tai khoan"
git push -u origin 2314299-LVDuc-Dang-Ky
```
Nhắn trưởng nhóm Tiến qua Zalo để Tiến review và merge vào `main`.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2314299-LVDuc-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng: Đăng nhập Email/Mật khẩu + Rate Limiting (FR-AUTH-002)

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

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-002 dang nhap email rate limiting"
git push -u origin 2314299-LVDuc-Dang-Nhap
```
Nhắn trưởng nhóm Tiến qua Zalo để Tiến review và merge vào `main`.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên một nhánh riêng.

---

### Chức năng 1: Đăng xuất & Thu hồi phiên làm việc (FR-AUTH-005)

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

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-005 dang xuat thu hoi token"
git push -u origin 2314299-LVDuc-Dang-Xuat
```

---

### Chức năng 2: Refresh Token Rotation (FR-AUTH-004)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2314299-LVDuc-Refresh-Token
```

#### Bước 2: Hiện thực Backend
1. Thư mục `Features/Auth/Commands/RefreshToken/`:
   - `RefreshTokenCommand(string RefreshToken)`: `IRequest<AuthResponseDto>`
   - `RefreshTokenCommandHandler`:
     - Tìm Refresh Token theo hash SHA-256.
     - Nếu token đã bị thu hồi (`IsRevoked`): Phát hiện tấn công tái sử dụng (Reuse Detection) -> thu hồi toàn bộ token family của user đó -> ném `UnauthorizedException("Token không hợp lệ.")`.
     - Nếu hợp lệ: Đánh dấu token cũ `Revoke()`, sinh cặp Access Token + Refresh Token mới, lưu CSDL và trả về `AuthResponseDto`.
2. Đăng ký endpoint `POST /api/v1/auth/refresh` trong `AuthEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Cấu hình Axios Interceptor trong `src/Frontend/lib/api/client.ts`:
   - Bắt mã lỗi 401 khi Access Token hết hạn.
   - Tự động gọi `POST /api/v1/auth/refresh` với refresh token đang lưu.
   - Cập nhật access token mới và retry lại request cũ một cách mượt mà, không làm gián đoạn người dùng.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-004 refresh token rotation"
git push -u origin 2314299-LVDuc-Refresh-Token
```

---

## 7. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Đăng ký tài khoản mới thành công (thử trên Scalar `http://localhost:5000/scalar/v1` hoặc giao diện web).
- [ ] Đăng nhập đúng mật khẩu trả về Access Token + Refresh Token; sai mật khẩu trả về lỗi 401 rõ ràng.
- [ ] Navbar hiển thị đúng trạng thái trước và sau khi đăng nhập.
- [ ] Đăng xuất và refresh token hoạt động trơn tru.
- [ ] Các nhánh chức năng đã được đẩy lên GitHub và merge vào `main`.
