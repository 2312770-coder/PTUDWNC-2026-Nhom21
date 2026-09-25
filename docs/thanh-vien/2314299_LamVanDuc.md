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

Tuần 5:
  🔲 FR-AUTH-004 — Làm mới Access Token (Refresh Token Rotation + Reuse Detection)

Tuần 6:
  🔲 FR-AUTH-003 — Đăng nhập bên thứ ba Google OAuth 2.0
  🔲 FR-RCP-005 — Xuất bản / Hủy xuất bản công thức (Tuân thủ điều kiện D11: >= 1 bước & >= 1 nguyên liệu)

Tuần 7:
  🔲 FR-JOB-001 — Hangfire background job tự động gửi email chào mừng qua MailHog

Tuần 8:
  🔲 Triển khai Production, Tinh chỉnh bảo mật luồng Auth, Guard Route Next.js, Hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

### Chức năng: Đăng ký tài khoản (FR-AUTH-001)
- **Nhánh**: `2314299-LVDuc-Dang-Ky` (Đã merge vào `main`)
- **Backend**:
  - `RegisterCommandHandler`: Kiểm tra email trùng, tạo user qua ASP.NET Core Identity, gán role "Author", sinh JWT Access Token và Refresh Token lưu vào database.
  - Validator `RegisterCommandValidator`: Kiểm tra định dạng email và mật khẩu $\ge 8$ ký tự.
- **Frontend**:
  - Trang `app/(auth)/register/page.tsx`: Form nhập Email, DisplayName, Password, Confirm Password có validate trực quan và liên kết chuyển trang đăng nhập.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

### Chức năng: Đăng nhập Email/Mật khẩu + Rate Limiting (FR-AUTH-002)
- **Nhánh**: `2314299-LVDuc-Dang-Nhap` (Đã merge vào `main`)
- **Backend**:
  - `LoginCommandHandler`: Kiểm tra tài khoản bị khóa (`IsLockedOutAsync`), kiểm tra mật khẩu, xử lý đếm số lần đăng nhập sai và tự động khóa 15 phút sau 5 lần thất bại. Sinh Access Token + Refresh Token mới.
  - Cấu hình ASP.NET Core Rate Limiting 5 requests/phút trong `API/DependencyInjection.cs`.
- **Frontend**:
  - Trang `app/(auth)/login/page.tsx`: Form đăng nhập, lưu token, cập nhật trạng thái User trên Navbar.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Làm trên nhánh riêng `2314299-LVDuc-Dang-Xuat`.

### Chức năng trọng tâm: Đăng xuất & Thu hồi phiên làm việc (FR-AUTH-005)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2314299-LVDuc-Dang-Xuat
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Auth/Commands/Logout/`:
   - `LogoutCommand(string RefreshToken)`: `IRequest<bool>`
   - `LogoutCommandHandler`:
     - Tìm Refresh Token tương ứng trong bảng `RefreshTokens`.
     - Nếu tìm thấy, đánh dấu thu hồi `token.Revoke()` hoặc xóa khỏi database.
     - Lưu thay đổi qua `SaveChangesAsync(ct)`.
2. Đăng ký endpoint `POST /api/v1/auth/logout` trong `AuthEndpoints.cs` (yêu cầu xác thực `RequireAuthorization`).

#### Bước 3: Hiện thực Frontend
1. Mở component `src/Frontend/components/layout/Navbar.tsx`.
2. Khi người dùng bấm nút "Đăng xuất":
   - Gửi request `POST /api/v1/auth/logout` kèm refresh token.
   - Xóa Access Token và thông tin user khỏi Cookie/LocalStorage.
   - Cập nhật state đăng nhập của Navbar về trạng thái chưa đăng nhập.
   - Điều hướng người dùng về trang đăng nhập `/login` hoặc reload trang chủ.
