# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Lâm Văn Đức (MSSV: 2314299)
### Vai trò: Authentication & Recipe Publishing

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
| 2 | **FR-AUTH-002** | Đăng nhập Email/Mật khẩu | `Features/Auth/Commands/Login/*` | `app/(auth)/login/page.tsx` |
| 3 | **FR-AUTH-004** | Làm mới Access Token (Rotation) | `Features/Auth/Commands/RefreshToken/*` | `lib/api/client.ts` (Interceptor refresh) |
| 4 | **FR-AUTH-005** | Đăng xuất & Thu hồi token | `Features/Auth/Commands/Logout/*` | Nút Đăng xuất trên Navbar |
| 5 | **FR-AUTH-003** | Đăng nhập Google OAuth 2.0 | `Features/Auth/Commands/GoogleLogin/*` | Nút Google Sign-In trong form Login |
| 6 | **FR-RCP-005** | Xuất bản / Hủy xuất bản (D11) | `Features/Recipes/Commands/PublishRecipe/*` | Nút Xuất bản trong trang quản lý bài |
| 7 | **FR-JOB-001** | Gửi email chào mừng thành viên | `Infrastructure/Jobs/WelcomeEmailJob.cs` | Xem email gửi đến tại MailHog (:8025) |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 7)

```
Tuần 2 (Tuần này): FR-AUTH-001 (Đăng ký tài khoản) & FR-AUTH-002 (Đăng nhập Email/Mật khẩu)
Tuần 3: FR-AUTH-004 (Refresh token Rotation) & FR-AUTH-005 (Đăng xuất)
Tuần 4: FR-AUTH-003 (Đăng nhập Google OAuth 2.0)
Tuần 5: FR-RCP-005 (Xuất bản / Hủy xuất bản công thức - kiểm tra D11)
Tuần 6: FR-JOB-001 (Hangfire gửi mail chào mừng khi đăng ký qua MailHog)
Tuần 7: Kiểm thử toàn diện luồng Auth, phân quyền Guard trên UI & hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (TUẦN NÀY BẮT ĐẦU LÀM)

> ⚠️ **Git**: Bạn làm việc trên **nhánh cá nhân** `2314299-LamVanDuc`. **Không tự merge vào `main`** — 

### Thiết lập nhánh cá nhân (lần đầu)
```powershell
git checkout -b 2314299-LamVanDuc
git push -u origin 2314299-LamVanDuc
```
*Từ những lần sau chỉ cần:* `git checkout 2314299-LamVanDuc`

### Trước khi code mỗi ngày — đồng bộ với `main`
```powershell
git fetch origin
git merge origin/main
```

---

### Chức năng 1: Đăng ký tài khoản (FR-AUTH-001)

#### Bước 1: Chuyển sang nhánh cá nhân của bạn
```powershell
git checkout 2314299-LamVanDuc
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
3. Kiểm tra validation trong `RegisterCommandValidator.cs` (Email hợp lệ, Mật khẩu $\ge 8$ ký tự).

#### Bước 3: Hiện thực Frontend
1. Mở file `src/Frontend/app/(auth)/register/page.tsx`.
2. Dựng form đăng ký: Email, Tên hiển thị, Tên đăng nhập (tùy chọn), Mật khẩu, Xác nhận mật khẩu.
3. Khi submit, gọi `POST /api/v1/auth/register`, nếu thành công lưu token và chuyển hướng về trang chủ.

#### Bước 4: Commit và push
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-001 dang ky tai khoan"
git push origin 2314299-LamVanDuc
```


---

### Chức năng 2: Đăng nhập bằng Email/Mật khẩu (FR-AUTH-002)

#### Bước 1: Tiếp tục trên nhánh cá nhân
```powershell
git checkout 2314299-LamVanDuc
```

#### Bước 2: Hiện thực Backend & Frontend
1. Backend: Mở `LoginCommandHandler.cs` trong `Features/Auth/Commands/Login/`.
   - Tìm user theo email. Nếu không thấy hoặc tài khoản bị khóa -> ném `UnauthorizedException`.
   - Kiểm tra mật khẩu: `await _userManager.CheckPasswordAsync(user, request.Password)`.
   - Sinh cặp token mới, lưu refresh token, trả về `AuthResponseDto`.
2. Frontend: Mở `src/Frontend/app/(auth)/login/page.tsx`:
   - Dựng form đăng nhập (Email, Mật khẩu, checkbox Ghi nhớ đăng nhập).
   - Xử lý submit lưu Access Token vào cookie/localStorage.
   - Cập nhật trạng thái Navbar (hiện avatar thay vì nút Đăng nhập khi đã đăng nhập thành công).

#### Bước 3: Commit và push
```powershell
git add .
git commit -m "auth: hien thuc FR-AUTH-002 dang nhap email"
git push origin 2314299-LamVanDuc
```

---

## 5. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Đăng ký tài khoản mới thành công (thử trên Scalar `http://localhost:5000/scalar/v1` hoặc giao diện web).
- [ ] Đăng nhập đúng mật khẩu trả về Access Token + Refresh Token; sai mật khẩu trả về lỗi 401 rõ ràng.
- [ ] Navbar hiển thị đúng trạng thái trước và sau khi đăng nhập.
- [ ] Nhánh cá nhân `2314299-LamVanDuc` đã được đẩy lên GitHub.
- [ ] Backend và Frontend không có lỗi build/lint.

