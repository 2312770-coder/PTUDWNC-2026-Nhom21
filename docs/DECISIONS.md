# Quyết định xử lý các điểm mâu thuẫn trong SRS

Tài liệu SRS v1.0.0 có vài chỗ nói không khớp nhau giữa các chương. Đây là
cách cả nhóm thống nhất xử lý, code trong repo đã làm theo đúng các quyết định
này. Ai muốn đổi thì bàn với cả nhóm trước, vì đổi một mình sẽ gây lệch schema
với module của người khác.

---

## D1 - Xóa Recipe: hard delete hay soft delete?

**SRS mâu thuẫn:**
- FR-RCP-007 (mục 3.3) nói rõ: *"Đây là hard delete (không dùng soft delete
  pattern cho recipe)"*.
- Mục 7.1 lại quy định MỌI entity kế thừa `BaseEntity` đều có `IsDeleted` và
  dùng Global Query Filter.
- Mục 8.3 (bảng API) ghi `DELETE /recipes/{id}` là *"soft delete"*.

**Quyết định:** `BaseEntity` VẪN có `IsDeleted` và Global Query Filter (theo
mục 7.1, vì đây là quy định chung cho toàn bộ hệ thống). Riêng cách xóa recipe
thì **người phụ trách FR-RCP-007 chốt với nhóm trước khi code**, và ghi lại
lựa chọn vào đây. Hai phương án:

- Dùng `SoftDelete(entity)` — theo mục 7.1 và 8.3, dữ liệu vẫn khôi phục được.
- Dùng `Remove(entity)` — theo FR-RCP-007, cascade xóa luôn Steps/Ingredients/Images.

Gợi ý nghiêng về **soft delete**, vì 2/3 chỗ trong SRS nói vậy và an toàn hơn
khi người dùng lỡ tay xóa nhầm.

---

## D2 - Domain layer có được phụ thuộc NuGet không?

**SRS mâu thuẫn:** CONS-001 và mục 6.2 nói Domain *"không có NuGet
dependencies (chỉ .NET BCL)"*, nhưng mục 7.7 lại yêu cầu `ApplicationUser`
kế thừa `IdentityUser` — mà class này nằm trong gói NuGet của ASP.NET Core Identity.

**Quyết định:** Domain tham chiếu đúng MỘT gói
`Microsoft.Extensions.Identity.Stores` để có `IdentityUser`. Ngoài gói đó ra,
Domain không thêm NuGet nào khác (không EF Core, không MediatR).

---

## D3 - Tên field: TimerMinutes/DurationMinutes, OrderIndex/SortOrder

**SRS mâu thuẫn:** phần mô tả nghiệp vụ ở chương 3 dùng tên khác với bảng
dữ liệu ở chương 7.

**Quyết định:** lấy theo **chương 7** (chương đặc tả dữ liệu, chi tiết và có
kiểu dữ liệu rõ ràng nhất):
- `RecipeStep.TimerMinutes` (không phải DurationMinutes)
- `RecipeIngredient.OrderIndex`, `RecipeImage.OrderIndex`, `Category.OrderIndex`
  (không phải SortOrder)
- `ApplicationUser.DisplayName` (không phải FullName)

---

## D4 - Cache: Redis hay IMemoryCache?

**SRS mâu thuẫn:** một số chỗ nhắc IMemoryCache, nhưng NFR-SCALE yêu cầu
backend phải scale-out được nhiều instance.

**Quyết định:** dùng **Redis** qua `ICacheService`/`RedisCacheService` cho mọi
dữ liệu cache dùng chung. Lý do: IMemoryCache nằm trong RAM của từng instance,
chạy 2 instance trở lên sẽ mỗi máy một dữ liệu khác nhau.

Cách dùng trong code: Query nào cần cache thì implement `ICacheable`, Command
nào làm dữ liệu thay đổi thì implement `ICacheInvalidator` — phần còn lại do
`CachingBehavior` và `CacheInvalidationBehavior` tự lo, Handler không cần
viết dòng cache nào.

---

## D5 - Hai image Docker phải đổi so với SRS mục 6.5

- **MinIO**: SRS ghi `minio/minio:latest`, nhưng Docker Hub đã chặn pull image
  này (*"pull access denied"*). Dùng
  `quay.io/minio/minio:RELEASE.2024-01-16T16-07-38Z` — cùng phần mềm, lấy từ
  registry chính chủ của MinIO.
- **Seq**: SRS ghi `datalust/seq:latest`, nhưng bản mới crash liên tục trên
  Docker Desktop Windows (lỗi `NativeDocumentStore.Migrate` do LMDB hỏng khi
  tắt máy đột ngột). Ghim `datalust/seq:2023.4` kèm `stop_grace_period: 30s`.
  Seq chỉ dùng cho dev, không deploy production nên không ảnh hưởng kiến trúc.

---

## D6 - Dev chạy HTTP cổng 5000, không dùng HTTPS

Theo SRS mục 5.2, Base URL môi trường dev là `http://localhost:5000/api/v1`.
HTTPS chỉ áp dụng ở production và do **Nginx** làm SSL termination (mục 6.5),
backend không tự chạy HTTPS. Vì vậy `app.UseHttpsRedirection()` được bọc trong
`if (!app.Environment.IsDevelopment())`.

---

## D7 - Rate Limiting đặt ở tầng API

Rate Limiting là middleware của tầng Presentation nên khai báo trong
`CulinaryBlog.API` (file `DependencyInjection.cs`, method `AddPresentation`),
không đặt ở Infrastructure. Chính sách giữ đúng SRS: `/auth/login` giới hạn
5 request/phút, vượt quá trả HTTP 429 kèm header `Retry-After` và `X-RateLimit-*`.
