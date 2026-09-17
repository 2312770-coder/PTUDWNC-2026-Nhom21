# GIÁO TRÌNH PHÁT TRIỂN ỨNG DỤNG WEB NÂNG CAO
### Phiên bản V4 · .NET 10 + Next.js App Router

---

# TÀI LIỆU ĐẶC TẢ YÊU CẦU PHẦN MỀM (SRS)
### Software Requirements Specification — Tiêu chuẩn IEEE 830 / ISO/IEC/IEEE 29148:2018
## Dự án: Blog Ẩm thực và Nấu ăn (Culinary Blog)

| Thuộc tính | Giá trị |
| :--- | :--- |
| **Phiên bản tài liệu** | 1.0.0 (Unified & Harmonized Edition) |
| **Ngày cập nhật** | 16/09/2026 |
| **Trạng thái** | Đã duyệt (Approved) |
| **Công nghệ Backend** | .NET 10 Minimal APIs, C# 13, EF Core 10 |
| **Công nghệ Frontend** | Next.js 15 App Router, TypeScript, Tailwind CSS |
| **Cơ sở dữ liệu** | PostgreSQL 16 |
| **Object Storage** | MinIO (S3-Compatible) |
| **Cache & Session** | Redis 7 |
| **Background Jobs** | Hangfire (PostgreSQL Storage) |

---

## LỊCH SỬ THAY ĐỔI TÀI LIỆU

| Phiên bản | Ngày | Tác giả / Vai trò | Nội dung thay đổi | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **1.0.0 (Harmonized)** | 16/09/2026 | Senior BA / Lead Architect | Chuẩn hóa và đồng bộ các mâu thuẫn trong SRS: chốt Soft Delete cho Recipe/Category, chuẩn hóa sorting query (`sortBy` & `sortOrder`), chuẩn hóa StepNumber/Title/TimerMinutes cho RecipeStep, cho phép Nullable Quantity/Unit cho RecipeIngredient, chốt điều kiện Publish (bắt buộc cả step và ingredient), thống nhất HTTP status codes (400 validation, 409 conflict). | **Approved** |
| 1.0.0 | 04/06/2026 | Senior BA / Architect | Phát hành lần đầu – Bản hoàn chỉnh theo IEEE 830 / ISO 29148. | Approved |
| 0.9.0 | 20/05/2026 | Senior BA | Bổ sung Chương 7 (Data Model), Chương 8 (API Spec) và Phụ lục. | Under Review |
| 0.8.0 | 05/05/2026 | Senior BA | Hoàn thiện Chương 3 (FR), bổ sung FR-FILE, FR-JOB, FR-OBS. | Draft |
| 0.5.0 | 15/04/2026 | Senior BA | Phác thảo ban đầu: Chương 1–4 (skeleton). | Draft |

---

## TỔNG HỢP CÁC QUYẾT ĐỊNH CHUẨN HÓA (DECISION LOG)

Nhằm đảm bảo tính nhất quán tuyệt đối giữa các tầng Domain, Application, Database và API Spec, các quyết định chuẩn hóa sau đây đã được phê duyệt và tích hợp trực tiếp vào nội dung đặc tả bên dưới:

1. **Cơ chế Xóa Recipe & Category (Soft Delete)**: Thống nhất áp dụng cơ chế **Soft Delete** (`IsDeleted = true`). Khi xóa Recipe qua `DELETE /api/v1/recipes/{id}`, hệ thống không xóa vật lý bản ghi trong database và **không xóa ảnh trên MinIO ngay** nhằm bảo toàn dữ liệu và hỗ trợ khôi phục. Các query mặc định áp dụng EF Core Global Query Filter `.Where(x => !x.IsDeleted)`.
2. **Tham số Sắp xếp (Sorting Query Parameters)**: Thống nhất chuẩn API sử dụng cặp tham số tách rời: `sortBy={fieldName}&sortOrder={asc|desc}` (mặc định: `sortBy=createdAt&sortOrder=desc`). Tại tầng Application Query Handler, hệ thống đồng thời hỗ trợ cú pháp gộp `sort=-createdAt` / `sort=title` để đảm bảo tương thích linh hoạt.
3. **Quản lý Bước Thực hiện (RecipeStep)**:
   - `stepNumber`: Server tự động tính toán (`Max(stepNumber) + 1`) nếu client không truyền. Nếu client truyền, server hỗ trợ chèn và tự động renumber lại các bước phía sau để đảm bảo thứ tự luôn liên tục ($1, 2, 3...$).
   - `title`: Bổ sung trường `title` (bắt buộc, `varchar(200) NOT NULL`) vào DTO tạo/cập nhật bước theo đúng schema bảng `RecipeStep`.
   - `timerMinutes`: Thống nhất tên thuộc tính thời gian là **`timerMinutes`** (thay vì `durationMinutes`) trên toàn hệ thống.
4. **Quản lý Nguyên liệu (RecipeIngredient)**:
   - `quantity` và `unit`: Được phép **Nullable** (kiểu `decimal(10,3)?` và `varchar(50)?`) để hỗ trợ trường hợp gia vị/nguyên liệu nêm nếm "vừa đủ" hoặc không định lượng cụ thể.
   - Nhập liệu phân số: Frontend Next.js chịu trách nhiệm chuyển đổi các dạng phân số (ví dụ: `1/2` $\rightarrow$ `0.5`, `1/4` $\rightarrow$ `0.25`) trước khi gửi lên API.
   - Tên trường thứ tự: Thống nhất tên thuộc tính là **`orderIndex`** (thay vì `sortOrder`).
5. **Điều kiện Xuất bản Công thức (Publish Recipe)**: Để xuất bản (Publish) một công thức, bắt buộc Recipe phải có **ít nhất 1 nguyên liệu** (`Ingredients.Count > 0`) **VÀ** **ít nhất 1 bước thực hiện** (`Steps.Count > 0`). Nếu thiếu một trong hai điều kiện, API trả về mã lỗi `400 Bad Request` kèm Application Error Code `RECIPE_PUBLISH_INCOMPLETE`.
6. **Mã phản hồi HTTP Status Codes**:
   - Lỗi validation dữ liệu đầu vào (FluentValidation): Thống nhất trả về **HTTP 400 Bad Request** với định dạng RFC 7807 `ValidationProblemDetails`.
   - Xung đột phiên bản / Concurrency conflict (`RowVersion` mismatch): Trả về **HTTP 409 Conflict** (`RECIPE_CONCURRENCY_CONFLICT`).
7. **Thực thể RecipeImage**: Xác định rõ `RecipeImage` là một **thực thể/bảng độc lập** có quan hệ 1:N với `Recipe` (chỉ có `RecipeNutrition` mới là Owned Entity dạng cột nhúng trong bảng `Recipes`).
8. **Kiến trúc Cache**: Áp dụng **Redis Distributed Cache** (thông qua `IDistributedCache` hoặc .NET 10 HybridCache/OutputCache với Redis) cho toàn bộ shared state (Categories, Recipes, Search) nhằm đảm bảo Backend hoàn toàn Stateless và sẵn sàng scale ngang nhiều container.
9. **Tài khoản & Hồ sơ Người dùng**: Request đăng ký tài khoản hỗ trợ các trường `{ email, password, displayName, userName? }`. Thuộc tính tên hiển thị chuẩn trong toàn bộ DB và API là **`displayName`** (kết hợp với `UserName` kế thừa từ `IdentityUser`).

---

## MỤC LỤC

- [CHƯƠNG 1. GIỚI THIỆU](#chương-1-giới-thiệu)
  - [1.1. Mục đích Tài liệu](#11-mục-đích-tài-liệu)
  - [1.2. Phạm vi Sản phẩm](#12-phạm-vi-sản-phẩm)
  - [1.3. Định nghĩa, Từ viết tắt và Ký hiệu](#13-định-nghĩa-từ-viết-tắt-và-ký-hiệu)
  - [1.4. Tài liệu Tham chiếu](#14-tài-liệu-tham-chiếu)
  - [1.5. Tổng quan Tài liệu](#15-tổng-quan-tài-liệu)
- [CHƯƠNG 2. MÔ TẢ TỔNG QUAN HỆ THỐNG](#chương-2-mô-tả-tổng-quan-hệ-thống)
  - [2.1. Bối cảnh Sản phẩm](#21-bối-cảnh-sản-phẩm)
  - [2.2. Chức năng Sản phẩm Tổng quát](#22-chức-năng-sản-phẩm-tổng-quát)
  - [2.3. Các Lớp Người dùng và Đặc điểm](#23-các-lớp-người-dùng-và-đặc-điểm)
  - [2.4. Môi trường Vận hành](#24-môi-trường-vận-hành)
  - [2.5. Ràng buộc Thiết kế và Hiện thực](#25-ràng-buộc-thiết-kế-và-hiện-thực)
  - [2.6. Giả định và Phụ thuộc](#26-giả-định-và-phụ-thuộc)
- [CHƯƠNG 3. YÊU CẦU CHỨC NĂNG CHI TIẾT](#chương-3-yêu-cầu-chức-năng-chi-tiết)
  - [3.1. Module Xác thực và Quản lý Người dùng (FR-AUTH)](#31-module-xác-thực-và-quản-lý-người-dùng-fr-auth)
  - [3.2. Module Quản lý Danh mục (FR-CAT)](#32-module-quản-lý-danh-mục-fr-cat)
  - [3.3. Module Quản lý Công thức Nấu ăn (FR-RCP)](#33-module-quản-lý-công-thức-nấu-ăn-fr-rcp)
  - [3.4. Module Tìm kiếm và Phân trang (FR-SRCH)](#34-module-tìm-kiếm-và-phân-trang-fr-srch)
  - [3.5. Module Quản lý Tệp tin (FR-FILE)](#35-module-quản-lý-tệp-tin-fr-file)
  - [3.6. Module Background Jobs (FR-JOB)](#36-module-background-jobs-fr-job)
  - [3.7. Module Quan sát Hệ thống (FR-OBS)](#37-module-quan-sát-hệ-thống-fr-obs)
- [CHƯƠNG 4. YÊU CẦU PHI CHỨC NĂNG (NFR)](#chương-4-yêu-cầu-phi-chức-năng-nfr)
  - [4.1. Hiệu năng (NFR-PERF)](#41-hiệu-năng-nfr-perf)
  - [4.2. Bảo mật (NFR-SEC)](#42-bảo-mật-nfr-sec)
  - [4.3. Khả năng Sử dụng (NFR-USE)](#43-khả-năng-sử-dụng-nfr-use)
  - [4.4. Độ tin cậy (NFR-REL)](#44-độ-tin-cậy-nfr-rel)
  - [4.5. Khả năng Bảo trì (NFR-MAINT)](#45-khả-năng-bảo-trì-nfr-maint)
  - [4.6. Khả năng Mở rộng (NFR-SCALE)](#46-khả-năng-mở-rộng-nfr-scale)
  - [4.7. Tối ưu SEO (NFR-SEO)](#47-tối-ưu-seo-nfr-seo)
- [CHƯƠNG 5. YÊU CẦU GIAO DIỆN NGOÀI](#chương-5-yêu-cầu-giao-diện-ngoài)
  - [5.1. Giao diện Người dùng (UI Next.js Routes)](#51-giao-diện-người-dùng-ui-nextjs-routes)
  - [5.2. Giao diện Phần mềm – REST API](#52-giao-diện-phần-mềm--rest-api)
  - [5.3. Giao diện Dịch vụ Bên thứ ba](#53-giao-diện-dịch-vụ-bên-thứ-ba)
  - [5.4. Giao diện Phần cứng](#54-giao-diện-phần-cứng)
- [CHƯƠNG 6. KIẾN TRÚC HỆ THỐNG](#chương-6-kiến-trúc-hệ-thống)
  - [6.1. Tổng quan Kiến trúc](#61-tổng-quan-kiến-trúc)
  - [6.2. Kiến trúc Backend – Clean Architecture](#62-kiến-trúc-backend--clean-architecture)
  - [6.3. CQRS + MediatR Pipeline](#63-cqrs--mediatr-pipeline)
  - [6.4. Mô hình Quan hệ Thực thể (ERD tóm tắt)](#64-mô-hình-quan-hệ-thực-thể-erd-tóm-tắt)
  - [6.5. Triển khai – Docker Compose](#65-triển-khai--docker-compose)
- [CHƯƠNG 7. MÔ HÌNH DỮ LIỆU](#chương-7-mô-hình-dữ-liệu)
  - [7.1. BaseEntity (Abstract)](#71-baseentity-abstract)
  - [7.2. Recipe](#72-recipe)
  - [7.2.1. RecipeNutrition (Owned Entity)](#721-recipenutrition-owned-entity--cột-trong-bảng-recipes)
  - [7.3. RecipeStep](#73-recipestep)
  - [7.4. RecipeIngredient](#74-recipeingredient)
  - [7.5. RecipeImage](#75-recipeimage)
  - [7.6. Category](#76-category)
  - [7.7. ApplicationUser (extends IdentityUser)](#77-applicationuser-extends-identityuser)
  - [7.8. RefreshToken](#78-refreshtoken)
- [CHƯƠNG 8. ĐẶC TẢ REST API](#chương-8-đặc-tả-rest-api)
  - [8.1. Authentication Module (/auth)](#81-authentication-module-auth)
  - [8.2. Categories Module (/categories)](#82-categories-module-categories)
  - [8.3. Recipes Module (/recipes)](#83-recipes-module-recipes)
  - [8.4. Recipe Images (/recipes/{id}/images)](#84-recipe-images-recipesidimages)
  - [8.5. Recipe Steps (/recipes/{id}/steps)](#85-recipe-steps-recipesidsteps)
  - [8.6. Recipe Ingredients (/recipes/{id}/ingredients)](#86-recipe-ingredients-recipesidingredients)
  - [8.7. Health Check Endpoints](#87-health-check-endpoints)
- [PHỤ LỤC](#phụ-lục)
  - [Phụ lục A – HTTP Status Codes](#phụ-lục-a--http-status-codes)
  - [Phụ lục B – Application Error Codes](#phụ-lục-b--application-error-codes)
  - [Phụ lục C – Từ điển Thuật ngữ](#phụ-lục-c--từ-điển-thuật-ngữ)

---

# CHƯƠNG 1. GIỚI THIỆU

## 1.1. Mục đích Tài liệu
Tài liệu Đặc tả Yêu cầu Phần mềm (Software Requirements Specification – SRS) này được biên soạn theo tiêu chuẩn IEEE 830-1998 và ISO/IEC/IEEE 29148:2018 nhằm mô tả đầy đủ, chính xác và nhất quán toàn bộ yêu cầu chức năng (Functional Requirements) và yêu cầu phi chức năng (Non-Functional Requirements) của dự án ứng dụng web **Blog Ẩm thực và Nấu ăn (Culinary Blog)**.

Tài liệu này phục vụ các đối tượng sau:
- **Nhóm phát triển Backend (.NET 10/C#):** Căn cứ thiết kế API, domain model, logic nghiệp vụ và kiến trúc Clean Architecture.
- **Nhóm phát triển Frontend (Next.js/TypeScript):** Căn cứ thiết kế giao diện, luồng người dùng, routing và tích hợp API.
- **Kỹ sư Kiểm thử (QA/QC):** Cơ sở xây dựng test cases, kiểm thử tích hợp và kiểm thử chấp nhận (acceptance testing).
- **Kiến trúc sư Hệ thống:** Tham chiếu khi đưa ra quyết định kiến trúc (architecture decisions).
- **Giảng viên và Sinh viên:** Tài liệu học thuật mẫu cho dự án thực hành môn Phát triển Ứng dụng Web Nâng cao.
- **Stakeholder / Product Owner:** Phê duyệt phạm vi và nghiệm thu các tính năng hệ thống.

**Phạm vi hiệu lực:** Tài liệu phiên bản 1.0.0 (Harmonized) đóng vai trò là tài liệu nền tảng (baseline) cho toàn bộ vòng đời phát triển dự án.

## 1.2. Phạm vi Sản phẩm

### 1.2.1. Tên và Định danh
| Thuộc tính | Giá trị |
| :--- | :--- |
| **Tên sản phẩm** | Culinary Blog – Blog Ẩm thực và Nấu ăn |
| **Định danh dự án** | CULINARY-BLOG-V1 |
| **Loại hệ thống** | Ứng dụng Web Full-Stack (API-Driven Architecture) |
| **Phiên bản sản phẩm** | 1.0.0 |
| **Môi trường đích** | Cloud/On-premise (Docker Compose + Nginx) |

### 1.2.2. Mô tả Sản phẩm
Culinary Blog là nền tảng web cho phép người dùng chia sẻ, khám phá và lưu trữ các công thức nấu ăn từ nhiều nền ẩm thực khác nhau. Hệ thống bao gồm:
- **Nền tảng chia sẻ công thức:** Tác giả (Author) đăng tải công thức với hình ảnh, danh sách nguyên liệu chi tiết, hướng dẫn từng bước thực hiện và thông tin dinh dưỡng.
- **Tổ chức nội dung:** Phân loại công thức theo danh mục (Category), độ khó (Difficulty Level), thời gian chuẩn bị và nấu.
- **Tìm kiếm thông minh:** Full-Text Search tiếng Việt sử dụng PostgreSQL `tsvector`/`tsquery` kết hợp `unaccent` extension.
- **Bảo mật đa lớp:** Xác thực JWT stateless, phân quyền theo vai trò (RBAC) và theo tài nguyên (Resource-Based Authorization), đăng nhập bên thứ ba Google OAuth 2.0.
- **Tối ưu hiệu năng và SEO:** Redis distributed cache, Next.js App Router (ISR/SSR), Open Graph Protocol, Schema.org Recipe structured data.
- **Quan sát hệ thống:** Structured logging (Serilog), distributed tracing & metrics (OpenTelemetry), health check endpoints.

### 1.2.3. Những gì KHÔNG thuộc phạm vi (Out of Scope v1.0.0)
- Hệ thống bình luận (Comment System) và đánh giá sao (Rating System).
- Tính năng lưu/đánh dấu công thức yêu thích (Bookmark/Favorite).
- Thông báo real-time (SignalR/WebSocket).
- Ứng dụng di động native (iOS/Android).
- Thanh toán / Tính năng thương mại điện tử.
- Hệ thống nhắn tin trực tiếp giữa người dùng.
- GraphQL API.

## 1.3. Định nghĩa, Từ viết tắt và Ký hiệu
| Thuật ngữ / Viết tắt | Định nghĩa đầy đủ |
| :--- | :--- |
| **SRS** | Software Requirements Specification – Đặc tả Yêu cầu Phần mềm. |
| **FR / NFR** | Functional Requirement / Non-Functional Requirement. |
| **API / REST** | Application Programming Interface / Representational State Transfer. |
| **JWT** | JSON Web Token – Chuẩn token xác thực stateless (RFC 7519). |
| **RBAC** | Role-Based Access Control – Kiểm soát truy cập dựa trên vai trò. |
| **CQRS** | Command Query Responsibility Segregation – Tách biệt lệnh ghi và truy vấn đọc. |
| **DDD** | Domain-Driven Design – Thiết kế lấy domain làm trung tâm. |
| **ORM** | Object-Relational Mapper (Entity Framework Core). |
| **FTS** | Full-Text Search – Tìm kiếm toàn văn bản. |
| **ISR / SSR / SSG** | Incremental Static Regeneration / Server-Side Rendering / Static Site Generation. |
| **LCP / CLS / INP** | Google Core Web Vitals (Largest Contentful Paint, Cumulative Layout Shift, Interaction to Next Paint). |
| **RFC 7807** | Chuẩn format chi tiết lỗi cho HTTP APIs (`application/problem+json`). |
| **Soft Delete** | Đánh dấu xóa logic bằng cờ `IsDeleted = true` thay vì xóa vật lý khỏi database. |

## 1.4. Tài liệu Tham chiếu
1. **IEEE Std 830-1998**: Recommended Practice for Software Requirements Specifications.
2. **ISO/IEC/IEEE 29148:2018**: Systems and software engineering — Life cycle processes — Requirements engineering.
3. **OWASP Top 10:2021**: The Ten Most Critical Web Application Security Risks.
4. **RFC 7807**: Problem Details for HTTP APIs.
5. **RFC 7519**: JSON Web Token (JWT).
6. **RFC 6749**: The OAuth 2.0 Authorization Framework.
7. **Microsoft Learn**: .NET 10 Minimal APIs, EF Core 10, ASP.NET Core Identity.
8. **Next.js Documentation**: Next.js 15 App Router & Server Components.
9. **PostgreSQL 16 Documentation**: Full-Text Search and Unaccent Extension.
10. **Schema.org**: Recipe Structured Data Specification.

## 1.5. Tổng quan Tài liệu
Tài liệu gồm 8 chương chính và 3 phụ lục, được trình bày chi tiết từ kiến trúc mức cao, yêu cầu chức năng, phi chức năng đến chi tiết mô hình dữ liệu và đặc tả REST API.

---

# CHƯƠNG 2. MÔ TẢ TỔNG QUAN HỆ THỐNG

## 2.1. Bối cảnh Sản phẩm

### 2.1.1. Vị trí trong Hệ sinh thái
Culinary Blog vận hành theo mô hình API-Driven Architecture. Backend (.NET 10 Minimal API) và Frontend (Next.js 15 App Router) là hai hệ thống độc lập, giao tiếp thông qua HTTP/JSON RESTful API.

```
┌─────────────────────────────────────────────────────────────────┐
│                      CULINARY BLOG SYSTEM                       │
│                                                                 │
│  ┌──────────────────┐               ┌────────────────────────┐  │
│  │ NEXT.JS FRONTEND │◄─────────────►│   .NET 10 BACKEND API  │  │
│  │   (App Router)   │   REST JSON   │ (Minimal APIs + Clean) │  │
│  │    Port: 3000    │               │       Port: 5000       │  │
│  └──────────────────┘               └───────────┬────────────┘  │
│                                                 │               │
│  ┌──────────┬──────────┬──────────┬─────────────┴─┬──────────┐  │
│  │PostgreSQL│  Redis   │  MinIO   │ Hangfire Jobs │GoogleAuth│  │
│  │  :5432   │  :6379   │  :9000   │  (Postgres)   │ OAuth2.0 │  │
│  └──────────┴──────────┴──────────┴───────────────┴──────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 2.1.2. Quan hệ với Hệ thống Ngoài
| Hệ thống Ngoài | Vai trò | Giao thức / Chuẩn | Hướng tích hợp |
| :--- | :--- | :--- | :--- |
| **PostgreSQL 16** | Hệ quản trị CSDL quan hệ chính | TCP + Npgsql Driver (EF Core 10) | Backend → PostgreSQL |
| **Redis 7** | Distributed Cache & Session Store | TCP + StackExchange.Redis | Backend → Redis |
| **MinIO (S3)** | Object Storage lưu trữ ảnh | HTTP/S3 API + AWS SDK for .NET | Backend → MinIO |
| **Google OAuth 2.0**| Đăng nhập bên thứ ba | HTTPS + OpenID Connect / PKCE | Client ↔ Google ↔ Backend |
| **Hangfire** | Quản lý Background Jobs in-process | PostgreSQL Job Storage | Backend (Internal) |
| **Serilog / Seq** | Logging tập trung (Development) | HTTP Sink → Seq (:5341) | Backend → Seq |
| **OpenTelemetry** | Tracing & Metrics chuẩn OTLP | OTLP / gRPC | Backend → Collector |
| **Nginx** | Reverse Proxy, SSL Termination | HTTP / HTTPS | Client → Nginx → Apps |

## 2.2. Chức năng Sản phẩm Tổng quát
Hệ thống gồm 27 Yêu cầu Chức năng (FR) được tổ chức thành 7 modules:
1. **Module Xác thực & Quản lý Người dùng (FR-AUTH - 7 FR):** Đăng ký, đăng nhập email/mật khẩu, đăng nhập Google OAuth 2.0, refresh token rotation, logout, xem/sửa thông tin cá nhân.
2. **Module Quản lý Danh mục (FR-CAT - 5 FR):** Xem danh sách danh mục, xem chi tiết danh mục kèm recipes, CRUD danh mục (Admin).
3. **Module Quản lý Công thức Nấu ăn (FR-RCP - 10 FR):** Xem danh sách công thức (phân trang, lọc, sắp xếp), xem chi tiết, tạo mới, cập nhật thông tin, xuất bản/hủy xuất bản, lưu trữ (archive), xóa công thức (soft delete), quản lý ảnh, nguyên liệu và các bước thực hiện.
4. **Module Tìm kiếm & Phân trang (FR-SRCH - 4 FR):** Full-Text Search tiếng Việt không dấu (tsvector/unaccent), lọc đa tiêu chí, sắp xếp linh hoạt, phân trang offset.
5. **Module Quản lý Tệp tin (FR-FILE - 2 FR):** Upload và xóa ảnh trên MinIO S3-compatible, kiểm tra MIME type và magic bytes.
6. **Module Background Jobs (FR-JOB - 3 FR):** Gửi email chào mừng, tạo thumbnail ảnh tự động, tạo sitemap.xml định kỳ hàng ngày.
7. **Module Quan sát Hệ thống (FR-OBS - 3 FR):** Liveness & Readiness health checks, structured logging Serilog, distributed tracing OpenTelemetry.

## 2.3. Các Lớp Người dùng và Đặc điểm
| Vai trò | Mô tả | Điều kiện | Quyền hạn chính |
| :--- | :--- | :--- | :--- |
| **Khách (Guest)** | Người dùng vãng lai chưa đăng nhập | Không cần tài khoản | Xem danh sách & chi tiết recipe (Published), xem danh mục, tìm kiếm công thức. |
| **Tác giả (Author)** | Người dùng đã đăng ký tài khoản | Có tài khoản & JWT hợp lệ | Toàn bộ quyền của Guest + Tạo recipe, chỉnh sửa/xóa/archive/publish recipe của chính mình (`AuthorId == currentUserId`), upload ảnh. |
| **Quản trị viên (Admin)** | Người quản trị hệ thống | Tài khoản có Role `Admin` | Toàn bộ quyền của Author + CRUD danh mục, chỉnh sửa/xóa bất kỳ recipe nào, truy cập Hangfire Dashboard, xem logs. |

*Phân quyền áp dụng 3 tầng: Role-Based (Guest/Author/Admin), Resource-Based (`AuthorId == currentUserId`), và Policy-Based.*

## 2.4. Môi trường Vận hành

### 2.4.1. Môi trường Server (Production)
- **Hệ điều hành:** Linux Ubuntu 22.04 LTS hoặc Debian 12 (chạy Docker).
- **Backend Runtime:** .NET 10 Runtime qua Docker image `mcr.microsoft.com/dotnet/aspnet:10.0`.
- **Frontend Runtime:** Node.js 20+ LTS (standalone output container).
- **Dịch vụ hỗ trợ:** PostgreSQL 16 (extensions `unaccent`, `pg_trgm`), Redis 7.2 (AOF persistence), MinIO S3.
- **Tài nguyên tối thiểu:** 2 vCPU, 4 GB RAM, 20 GB SSD.

### 2.4.2. Môi trường Phát triển (Development)
- .NET 10 SDK (`dotnet --version` 10.0.x).
- Node.js 20+ LTS và npm 10+.
- Docker Desktop 4.x+ chạy PostgreSQL, Redis, MinIO, Seq (`datalust/seq:2023.4`), Mailhog.
- IDE: Visual Studio 2022 v17.12+, JetBrains Rider 2024+, hoặc VS Code với C# Dev Kit.
- Base URL Backend local: `http://localhost:5000/api/v1`. Tài liệu API trực quan: `/scalar`.

## 2.5. Ràng buộc Thiết kế và Hiện thực (Design Constraints)
- **CONS-001 (Kiến trúc):** Backend bắt buộc tuân thủ **Clean Architecture** (4 tầng: Domain, Application, Infrastructure, Presentation). Tầng Domain độc lập hoàn toàn, chỉ tham chiếu `Microsoft.Extensions.Identity.Stores` để kế thừa `IdentityUser`.
- **CONS-002 (Pattern):** CQRS kết hợp MediatR là bắt buộc cho tầng Application. Mỗi use case là một Command/Query và Handler riêng biệt.
- **CONS-003 (Framework):** Backend dùng .NET 10 Minimal APIs (không dùng MVC Controllers). Frontend dùng Next.js App Router.
- **CONS-004 (Bảo mật):** Xác thực JWT stateless (Access Token 15 phút, Refresh Token 7 ngày với cơ chế Token Rotation và Reuse Detection). Mật khẩu hash PBKDF2 qua Identity.
- **CONS-005 (API):** Tuân thủ chuẩn RESTful, trả về lỗi theo RFC 7807 (`application/problem+json`). Versioning URL `/api/v1/`.
- **CONS-006 (Database):** PostgreSQL 16 là DBMS duy nhất. Migration qua EF Core Code-First.
- **CONS-007 (File Upload):** Dung lượng tối đa 5 MB. Chỉ chấp nhận `image/jpeg`, `image/png`, `image/webp`, `image/avif`. Bắt buộc kiểm tra MIME type và Magic Bytes.
- **CONS-008 (Validation):** Input validation tập trung tại Application Layer qua FluentValidation và MediatR Pipeline Behavior.
- **CONS-009 (Container):** Ứng dụng được đóng gói qua Dockerfile multi-stage build và chạy bằng Docker Compose.
- **CONS-010 (Logging):** Structured logging Serilog với `CorrelationId`, `RequestPath`, `UserId`.

## 2.6. Giả định và Phụ thuộc Bên ngoài
- Dữ liệu seed ban đầu: thư viện Bogus sinh 50 recipes và 5 tác giả mẫu.
- Thư viện/Dịch vụ ngoài: Google OAuth 2.0 API, MinIO S3, Redis 7, SendGrid/SMTP Mailhog. Khi dịch vụ ngoài gặp sự cố, hệ thống phải graceful degradation (ví dụ: mất Redis thì fallback query trực tiếp DB).

---

# CHƯƠNG 3. YÊU CẦU CHỨC NĂNG CHI TIẾT

## 3.1. Module Xác thực và Quản lý Người dùng (FR-AUTH)

### FR-AUTH-001: Đăng ký Tài khoản Mới (User Registration)
- **Mã yêu cầu:** `FR-AUTH-001` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Guest
- **Mô tả:** Đăng ký tài khoản tác giả bằng email, mật khẩu và tên hiển thị. Tự động gán role "Author", kích hoạt phiên đăng nhập (trả về cặp JWT tokens) và đẩy job gửi email chào mừng vào Hangfire.
- **Điều kiện tiên quyết:** Email chưa tồn tại trong hệ thống.
- **HTTP Endpoint:** `POST /api/v1/auth/register`
- **Request Body:**
  ```json
  {
    "email": "user@example.com",
    "password": "Password123@",
    "displayName": "Nguyễn Văn A",
    "userName": "nguyenvana"
  }
  ```
  *(Ghi chú: Nếu không truyền `userName`, server tự động sinh từ phần prefix của email).*
- **Luồng chính:**
  1. Client gửi `POST /api/v1/auth/register`.
  2. `ValidationBehavior` kiểm tra format email, độ mạnh mật khẩu ($\ge 8$ ký tự, có hoa, thường, số, ký tự đặc biệt), `displayName` (2–100 ký tự).
  3. Kiểm tra email không trùng qua `UserManager.FindByEmailAsync`.
  4. Tạo `ApplicationUser`, gán role `Author`, hash password qua PBKDF2.
  5. Sinh cặp JWT Access Token (15 phút) và Refresh Token (7 ngày, lưu SHA-256 hash vào bảng `RefreshTokens`).
  6. Enqueue Hangfire background job gửi email chào mừng (`WelcomeEmailJob`).
  7. Trả về HTTP `201 Created` kèm `AuthResponseDto`.
- **Luồng ngoại lệ:**
  - Email đã tồn tại: Trả về HTTP `409 Conflict` (`AUTH_EMAIL_EXISTS`).
  - Dữ liệu không hợp lệ: Trả về HTTP `400 Bad Request` (`VALIDATION_ERROR`).

### FR-AUTH-002: Đăng nhập bằng Email/Mật khẩu (Local Login)
- **Mã yêu cầu:** `FR-AUTH-002` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author, Admin
- **Mô tả:** Đăng nhập hệ thống bằng email và mật khẩu đã đăng ký.
- **HTTP Endpoint:** `POST /api/v1/auth/login`
- **Request Body:** `{ "email": "user@example.com", "password": "Password123@" }`
- **Luồng chính:**
  1. Kiểm tra tài khoản tồn tại và không bị khóa (`LockoutEnabled`).
  2. Xác minh mật khẩu bằng `UserManager.CheckPasswordAsync`.
  3. Reset số lần đăng nhập sai (`ResetAccessFailedCountAsync`).
  4. Sinh cặp Access Token và Refresh Token mới, lưu DB.
  5. Trả về HTTP `200 OK` kèm `AuthResponseDto`.
- **Luồng ngoại lệ:**
  - Sai tài khoản/mật khẩu: HTTP `401 Unauthorized` (`AUTH_INVALID_CREDENTIALS`, thông báo chung để tránh user enumeration).
  - Đăng nhập sai quá 5 lần: Tài khoản bị khóa 15 phút, trả về HTTP `423 Locked`.
  - Tài khoản bị vô hiệu hóa (`IsActive = false`): Trả về HTTP `403 Forbidden` (`AUTH_ACCOUNT_DISABLED`).

### FR-AUTH-003: Đăng nhập bằng Google OAuth 2.0
- **Mã yêu cầu:** `FR-AUTH-003` | **Mức ưu tiên:** Should Have (S) | **Tác nhân:** Guest / Author
- **Mô tả:** Xác thực người dùng qua Google OpenID Connect (ID Token). Nếu email chưa có trong hệ thống thì tự động tạo tài khoản với role `Author`. Nếu đã có thì liên kết Google Login Provider.
- **HTTP Endpoint:** `POST /api/v1/auth/google`
- **Request Body:** `{ "idToken": "<google_id_token>" }`
- **Phản hồi:** HTTP `200 OK` kèm `AuthResponseDto`. Lỗi token: HTTP `400 Bad Request` (`AUTH_GOOGLE_TOKEN_INVALID`).

### FR-AUTH-004: Làm mới Access Token (Token Refresh)
- **Mã yêu cầu:** `FR-AUTH-004` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Client
- **Mô tả:** Đổi refresh token còn hạn để lấy cặp token mới mà không cần đăng nhập lại. Áp dụng cơ chế **Refresh Token Rotation**.
- **HTTP Endpoint:** `POST /api/v1/auth/refresh`
- **Request Body:** `{ "refreshToken": "<raw_refresh_token>" }`
- **Quy tắc bảo mật (Reuse Detection):**
  - Hash raw token bằng SHA-256 để tìm trong database.
  - Nếu token đã bị thu hồi (`IsRevoked = true`): Phát hiện tái sử dụng trái phép (Token Reuse Attack) $\rightarrow$ Ngay lập tức thu hồi toàn bộ các refresh token thuộc token family của user đó $\rightarrow$ Trả về HTTP `401 Unauthorized` (`AUTH_REFRESH_TOKEN_REVOKED`).
  - Nếu hợp lệ: Đánh dấu token cũ `IsRevoked = true`, `RevokedAt = UtcNow`, cấp token mới. Trả về HTTP `200 OK`.

### FR-AUTH-005: Đăng xuất (Logout / Token Revocation)
- **Mã yêu cầu:** `FR-AUTH-005` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author, Admin
- **Mô tả:** Thu hồi refresh token trong database để vô hiệu hóa phiên làm việc.
- **HTTP Endpoint:** `POST /api/v1/auth/logout` (Header: `Authorization: Bearer <token>`)
- **Request Body:** `{ "refreshToken": "<raw_refresh_token>" }`
- **Phản hồi:** HTTP `204 No Content` (idempotent, không tiết lộ trạng thái token).

### FR-AUTH-006: Xem Hồ sơ Cá nhân (View Profile)
- **Mã yêu cầu:** `FR-AUTH-006` | **Mức ưu tiên:** Should Have (S) | **Tác nhân:** Author, Admin
- **HTTP Endpoint:** `GET /api/v1/auth/me`
- **Phản hồi:** HTTP `200 OK` với DTO: `{ id, email, userName, displayName, avatarUrl, bio, roles, createdAt }`. Không bao giờ trả về PasswordHash hay SecurityStamp.

### FR-AUTH-007: Cập nhật Hồ sơ Cá nhân (Update Profile)
- **Mã yêu cầu:** `FR-AUTH-007` | **Mức ưu tiên:** Should Have (S) | **Tác nhân:** Author, Admin
- **Mô tả:** Cập nhật thông tin `displayName`, `avatarUrl`, `bio`. Email và UserName không đổi qua endpoint này.
- **HTTP Endpoint:** `PATCH /api/v1/auth/me`
- **Request Body:** `{ "displayName"?: "...", "avatarUrl"?: "...", "bio"?: "..." }`
- **Phản hồi:** HTTP `200 OK` với profile đã cập nhật.

---

## 3.2. Module Quản lý Danh mục (FR-CAT)

### FR-CAT-001: Xem Danh sách Danh mục
- **Mã yêu cầu:** `FR-CAT-001` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Tất cả
- **Mô tả:** Lấy danh sách tất cả danh mục kèm số lượng recipes đã xuất bản (`recipeCount`). Kết quả được cache trên Redis.
- **HTTP Endpoint:** `GET /api/v1/categories`
- **Phản hồi:** HTTP `200 OK` mảng danh mục: `[{ id, name, slug, description, imageUrl, orderIndex, recipeCount }]`.

### FR-CAT-002: Xem Chi tiết Danh mục và Công thức
- **Mã yêu cầu:** `FR-CAT-002` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Tất cả
- **HTTP Endpoint:** `GET /api/v1/categories/{slug}?page=1&pageSize=12&sortBy=createdAt&sortOrder=desc`
- **Mô tả:** Trả về thông tin danh mục theo slug kèm danh sách phân trang các Published recipes thuộc danh mục đó.
- **Phản hồi:** HTTP `200 OK` với `{ category: {...}, recipes: PagedResult<RecipeSummaryDto> }`. Không tìm thấy: HTTP `404 Not Found`.

### FR-CAT-003: Tạo Danh mục Mới [Admin]
- **Mã yêu cầu:** `FR-CAT-003` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Admin
- **Mô tả:** Admin tạo danh mục mới. Slug được tự động sinh từ Name (slugify tiếng Việt). Nếu trùng, tự động gắn suffix `-2`, `-3`. Sau khi tạo, invalidate cache danh mục trên Redis.
- **HTTP Endpoint:** `POST /api/v1/categories` (Role: Admin)
- **Request Body:** `{ "name": "Món Tráng Miệng", "description": "...", "imageUrl": "...", "orderIndex": 1 }`
- **Phản hồi:** HTTP `201 Created` kèm `CategoryDto` và `Location` header. Trùng tên: HTTP `409 Conflict` (`CATEGORY_NAME_EXISTS`).

### FR-CAT-004: Cập nhật Danh mục [Admin]
- **Mã yêu cầu:** `FR-CAT-004` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Admin
- **Mô tả:** Cập nhật thông tin danh mục. Slug được giữ nguyên để bảo toàn SEO URL. Invalidate cache Redis.
- **HTTP Endpoint:** `PUT /api/v1/categories/{id}`
- **Request Body:** `{ "name": "...", "description": "...", "imageUrl": "...", "orderIndex": 1 }`
- **Phản hồi:** HTTP `200 OK`. Không tìm thấy: HTTP `404 Not Found`.

### FR-CAT-005: Xóa Danh mục [Admin]
- **Mã yêu cầu:** `FR-CAT-005` | **Mức ưu tiên:** Should Have (S) | **Tác nhân:** Admin
- **Mô tả:** Xóa danh mục bằng cơ chế **Soft Delete** (`IsDeleted = true`). Quy tắc nghiệp vụ bắt buộc: Không được xóa danh mục nếu đang có công thức trực thuộc (dù là Draft hay Published). Admin phải chuyển toàn bộ recipe sang danh mục khác trước khi xóa.
- **HTTP Endpoint:** `DELETE /api/v1/categories/{id}`
- **Phản hồi:**
  - Thành công: HTTP `204 No Content`.
  - Danh mục còn chứa công thức: HTTP `409 Conflict` (`CATEGORY_DELETE_HAS_RECIPES`).

---

## 3.3. Module Quản lý Công thức Nấu ăn (FR-RCP)

### FR-RCP-001: Xem Danh sách Công thức (Paginated + Filtered + Sorted)
- **Mã yêu cầu:** `FR-RCP-001` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Tất cả
- **Mô tả:** Xem danh sách công thức có phân trang, lọc và sắp xếp.
  - Phân quyền hiển thị: Guest chỉ thấy recipe `Published`. Author thấy thêm `Draft` và `Archived` của chính mình. Admin thấy tất cả trạng thái.
  - Tham số sắp xếp chuẩn: `sortBy` (`createdAt`, `title`, `cookTime`, `prepTime`) và `sortOrder` (`asc`, `desc`). Đồng thời hỗ trợ tương thích `sort=-createdAt`.
  - Cache: Output Cache qua Redis (TTL 15 phút, vary by query string).
- **HTTP Endpoint:**
  `GET /api/v1/recipes?page=1&pageSize=12&categoryId={guid}&difficulty=Easy&maxCookTime=30&sortBy=createdAt&sortOrder=desc`
- **Phản hồi:** HTTP `200 OK` với cấu trúc `PagedResult<RecipeSummaryDto>`.

### FR-RCP-002: Xem Chi tiết Công thức
- **Mã yêu cầu:** `FR-RCP-002` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Tất cả
- **Mô tả:** Lấy toàn bộ thông tin chi tiết của một công thức theo `slug`: thông tin cơ bản, tác giả, danh mục, thông tin dinh dưỡng (`RecipeNutrition`), danh sách nguyên liệu (`RecipeIngredients` sắp xếp theo `orderIndex`), các bước thực hiện (`RecipeSteps` sắp xếp theo `stepNumber`), bộ sưu tập ảnh (`RecipeImages` sắp xếp theo `orderIndex`).
- **HTTP Endpoint:** `GET /api/v1/recipes/{slug}`
- **Quyền hạn:** Recipe `Draft`/`Archived` chỉ tác giả sở hữu hoặc Admin mới có quyền xem (nếu không có quyền trả về `403 Forbidden`).
- **Phản hồi:** HTTP `200 OK` với `RecipeDetailDto`.

### FR-RCP-003: Tạo Công thức Nấu ăn Mới [Author/Admin]
- **Mã yêu cầu:** `FR-RCP-003` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author, Admin
- **Mô tả:** Tạo công thức mới. Trạng thái ban đầu luôn là `Draft`. Slug được tự động sinh từ `title`. Có thể đính kèm luôn `steps`, `ingredients`, `nutrition` trong cùng request hoặc thêm qua các endpoint riêng.
- **HTTP Endpoint:** `POST /api/v1/recipes`
- **Request Body:**
  ```json
  {
    "title": "Phở bò truyền thống",
    "description": "Cách nấu phở bò đậm đà chuẩn vị Hà Nội...",
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "prepTimeMinutes": 30,
    "cookTimeMinutes": 180,
    "servings": 4,
    "difficulty": "Medium",
    "instructions": "Hướng dẫn chung...",
    "nutrition": {
      "calories": 450.5,
      "protein": 28.0,
      "carbohydrates": 52.0,
      "fat": 12.5,
      "fiber": 2.1,
      "sodium": 890.0
    },
    "ingredients": [
      { "name": "Bánh phở tươi", "quantity": 500, "unit": "gram", "orderIndex": 1 },
      { "name": "Hành lá", "quantity": null, "unit": null, "notes": "Cắt nhỏ vừa đủ", "orderIndex": 2 }
    ],
    "steps": [
      { "stepNumber": 1, "title": "Sơ chế xương", "description": "Rửa sạch xương ống chần qua nước sôi...", "timerMinutes": 15 }
    ]
  }
  ```
- **Phản hồi:** HTTP `201 Created` kèm `RecipeDto`. Trùng slug: HTTP `409 Conflict` (`RECIPE_SLUG_EXISTS`).

### FR-RCP-004: Cập nhật Công thức [Owner/Admin]
- **Mã yêu cầu:** `FR-RCP-004` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Cập nhật thông tin công thức. Kiểm soát đồng thời lạc quan (Optimistic Concurrency Control) bằng trường `RowVersion` (hoặc header `If-Match`). Nếu phát hiện dữ liệu đã bị sửa bởi phiên khác, trả về `409 Conflict`.
- **HTTP Endpoint:** `PUT /api/v1/recipes/{id}`
- **Request Body:**
  ```json
  {
    "title": "...",
    "description": "...",
    "categoryId": "...",
    "prepTimeMinutes": 20,
    "cookTimeMinutes": 45,
    "servings": 2,
    "difficulty": "Easy",
    "instructions": "...",
    "rowVersion": "AAAAAAAAB9M="
  }
  ```
- **Phản hồi:** HTTP `200 OK`. Xung đột phiên bản: HTTP `409 Conflict` (`RECIPE_CONCURRENCY_CONFLICT`). Không có quyền: HTTP `403 Forbidden`.

### FR-RCP-005: Xuất bản / Hủy Xuất bản Công thức (Publish / Unpublish)
- **Mã yêu cầu:** `FR-RCP-005` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Chuyển đổi trạng thái giữa `Draft` và `Published`.
- **Quy tắc nghiệp vụ xuất bản (Publish):** Bắt buộc Recipe phải có **ít nhất 1 nguyên liệu** (`Ingredients.Count > 0`) **VÀ** **ít nhất 1 bước thực hiện** (`Steps.Count > 0`). Khi xuất bản thành công, gán `PublishedAt = UtcNow`.
- **HTTP Endpoints:**
  - Xuất bản: `PATCH /api/v1/recipes/{id}/publish`
  - Hủy xuất bản: `PATCH /api/v1/recipes/{id}/unpublish`
- **Phản hồi:**
  - Thành công: HTTP `200 OK` kèm thông tin recipe.
  - Thiếu nguyên liệu hoặc bước khi publish: HTTP `400 Bad Request` (`RECIPE_PUBLISH_INCOMPLETE`).

### FR-RCP-006: Lưu trữ Công thức (Archive / Unarchive)
- **Mã yêu cầu:** `FR-RCP-006` | **Mức ưu tiên:** Should Have (S) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Chuyển trạng thái recipe thành `Archived` (ẩn khỏi danh sách công khai mà không xóa dữ liệu).
- **HTTP Endpoint:** `PATCH /api/v1/recipes/{id}/archive`
- **Phản hồi:** HTTP `200 OK`.

### FR-RCP-007: Xóa Công thức [Author sở hữu / Admin] — Soft Delete
- **Mã yêu cầu:** `FR-RCP-007` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Xóa công thức bằng cơ chế **Soft Delete** (`IsDeleted = true`). Bản ghi recipe và các liên kết con được giữ lại trong database nhưng bị lọc khỏi mọi truy vấn thông thường thông qua EF Core Global Query Filter. **Ảnh trên MinIO không bị xóa ngay** để có thể khôi phục khi cần thiết.
- **HTTP Endpoint:** `DELETE /api/v1/recipes/{id}`
- **Luồng xử lý:**
  1. Kiểm tra xác thực và kiểm tra quyền tác giả sở hữu (`AuthorId == currentUserId` hoặc role `Admin`).
  2. Đánh dấu `recipe.IsDeleted = true`, `recipe.UpdatedAt = DateTime.UtcNow`.
  3. Lưu thay đổi qua `SaveChangesAsync()`.
  4. Invalidate cache Redis cho tag `"recipes"` và slug tương ứng.
  5. Trả về HTTP `204 No Content`.

### FR-RCP-008: Quản lý Ảnh Công thức (Upload / Set Primary / Delete)
- **Mã yêu cầu:** `FR-RCP-008` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Quản lý thư viện ảnh của Recipe (lưu trên MinIO). Ảnh đầu tiên tải lên được tự động đặt làm ảnh chính (`IsPrimary = true`). Khi xóa ảnh chính, hệ thống tự động gán ảnh kế tiếp làm ảnh chính.
- **HTTP Endpoints:**
  - **Upload ảnh:** `POST /api/v1/recipes/{id}/images` (multipart/form-data: field `file`, `altText?`) $\rightarrow$ HTTP `201 Created`
  - **Đặt ảnh chính:** `PATCH /api/v1/recipes/{id}/images/{imageId}/primary` $\rightarrow$ HTTP `200 OK`
  - **Xóa ảnh:** `DELETE /api/v1/recipes/{id}/images/{imageId}` $\rightarrow$ Xóa record DB và đẩy Hangfire job xóa file vật lý trên MinIO $\rightarrow$ HTTP `204 No Content`.

### FR-RCP-009: Quản lý Nguyên liệu (CRUD RecipeIngredient)
- **Mã yêu cầu:** `FR-RCP-009` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Quản lý nguyên liệu nấu ăn. Hỗ trợ trường `quantity` và `unit` dạng Nullable (phục vụ gia vị nêm nếm vừa đủ). Thứ tự hiển thị quản lý qua trường `orderIndex`.
- **HTTP Endpoints:**
  - Thêm nguyên liệu: `POST /api/v1/recipes/{id}/ingredients`
    - Body: `{ "name": "Hành lá", "quantity": null, "unit": null, "notes": "Cắt khúc vừa ăn", "orderIndex": 1 }`
    - Response: HTTP `201 Created`
  - Sửa nguyên liệu: `PUT /api/v1/recipes/{id}/ingredients/{ingId}` $\rightarrow$ HTTP `200 OK`
  - Xóa nguyên liệu: `DELETE /api/v1/recipes/{id}/ingredients/{ingId}` $\rightarrow$ HTTP `204 No Content`

### FR-RCP-010: Quản lý Các bước Thực hiện (CRUD RecipeStep)
- **Mã yêu cầu:** `FR-RCP-010` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Author sở hữu / Admin
- **Mô tả:** Quản lý các bước nấu ăn.
  - `stepNumber`: Server tự động gán `Max(stepNumber) + 1` nếu không truyền. Khi xóa một bước, hệ thống tự động renumber lại các bước còn lại theo thứ tự $1, 2, 3...$.
  - Trường bắt buộc: `title` (Tên bước), `description` (Mô tả chi tiết).
  - Thuộc tính thời gian chuẩn hóa: **`timerMinutes`**.
- **HTTP Endpoints:**
  - Thêm bước: `POST /api/v1/recipes/{id}/steps`
    - Body: `{ "title": "Sơ chế rau củ", "description": "Rửa sạch và cắt lát...", "timerMinutes": 10, "imageUrl": "..." }`
    - Response: HTTP `201 Created` kèm `RecipeStepDto`.
  - Sửa bước: `PUT /api/v1/recipes/{id}/steps/{stepId}`
    - Body: `{ "stepNumber"?: 1, "title": "...", "description": "...", "timerMinutes"?: 15, "imageUrl"?: "..." }`
    - Response: HTTP `200 OK`.
  - Xóa bước: `DELETE /api/v1/recipes/{id}/steps/{stepId}`
    - Response: HTTP `204 No Content` (các bước sau được tự động renumber).

---

## 3.4. Module Tìm kiếm và Phân trang (FR-SRCH)

### FR-SRCH-001: Tìm kiếm Toàn văn bản (Full-Text Search)
- **Mã yêu cầu:** `FR-SRCH-001` | **Mức ưu tiên:** Must Have (M) | **Tác nhân:** Tất cả
- **Mô tả:** Tìm kiếm toàn văn bản trên công thức tiếng Việt bằng PostgreSQL `tsvector`/`tsquery` với extension `unaccent` (hỗ trợ tìm gõ không dấu: gõ "pho bo" tìm ra "phở bò"). Trường `SearchVector` được đánh GIN index. Kết quả được sắp xếp theo độ liên quan `ts_rank()`.
- **HTTP Endpoint:** `GET /api/v1/recipes/search?q={searchTerm}&page=1&pageSize=10`
- **Phản hồi:** HTTP `200 OK` với `PagedResult<RecipeSummaryDto>`.

### FR-SRCH-002/003/004: Lọc, Sắp xếp và Phân trang
- **Lọc (FR-SRCH-002):** Hỗ trợ lọc theo `categoryId`, `difficulty` (`Easy`, `Medium`, `Hard`, `Expert`), `maxCookTime`, `minServings`.
- **Sắp xếp (FR-SRCH-003):** Chuẩn hóa qua `sortBy` (`createdAt`, `title`, `cookTime`, `prepTime`) và `sortOrder` (`asc`, `desc`). Hỗ trợ tương thích tiền tố `-` (`sort=-createdAt`).
- **Phân trang (FR-SRCH-004):** Phân trang kiểu Offset (`page` mặc định 1, `pageSize` mặc định 12, tối đa 50). Trả về metadata: `totalCount`, `page`, `pageSize`, `totalPages`, `hasNextPage`, `hasPreviousPage`.

---

## 3.5. Module Quản lý Tệp tin (FR-FILE)

### FR-FILE-001: Upload Tệp tin lên MinIO S3
- **Mã yêu cầu:** `FR-FILE-001` | **Mức ưu tiên:** Must Have (M)
- **Mô tả:** Tải lên tệp ảnh qua interface trừu tượng `IFileStorageService`.
- **Ràng buộc kỹ thuật:**
  - Kích thước tối đa: **5 MB**.
  - MIME Type hợp lệ: `image/jpeg`, `image/png`, `image/webp`, `image/avif`.
  - Xác thực Magic Bytes thực tế (JPEG: `FF D8 FF`, PNG: `89 50 4E 47`, WebP: `52 49 46 46`).
  - Đặt tên tệp duy nhất dạng UUID: `recipes/{recipeId}/{Guid.NewGuid()}{ext}` để phòng chống Path Traversal.

### FR-FILE-002: Xóa Tệp tin khỏi MinIO
- **Mã yêu cầu:** `FR-FILE-002` | **Mức ưu tiên:** Must Have (M)
- **Mô tả:** Xóa tệp đối tượng trên MinIO qua `IFileStorageService.DeleteAsync`. Thực hiện bất đồng bộ qua Hangfire job để không làm nghẽn luồng HTTP request. Thao tác có tính chất idempotent (không ném lỗi nếu file không còn trên storage).

---

## 3.6. Module Background Jobs (FR-JOB)

| Mã FR | Tên Job | Phân loại | Trigger | Mô tả | Chính sách Retry |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **FR-JOB-001** | **Welcome Email Job** | Fire-and-forget | Ngay sau khi `FR-AUTH-001` thành công | Gửi email kích hoạt/chào mừng người dùng mới qua MailKit/SMTP. | Tự động retry 3 lần với exponential backoff (1m, 5m, 30m). |
| **FR-JOB-002** | **Image Resize / Thumbnail Job** | Fire-and-forget | Sau khi `FR-RCP-008` upload ảnh gốc | Sinh 2 phiên bản ảnh: Medium ($800 \times 600$) và Thumbnail ($300 \times 300$). Upload lên MinIO và update URLs vào DB. | Retry 3 lần. |
| **FR-JOB-003** | **Sitemap Generation Job** | Recurring (Cron) | Chạy định kỳ lúc 02:00 AM UTC hàng ngày (`0 2 * * *`) | Tạo tệp `sitemap.xml` chứa danh sách toàn bộ Published recipes, categories và static pages. Tự động ping Google Search Console. | Retry 2 lần. |

---

## 3.7. Module Quan sát Hệ thống (FR-OBS)

| Mã FR | Tên chức năng | Mô tả chi tiết | Công nghệ / Triển khai |
| :--- | :--- | :--- | :--- |
| **FR-OBS-001** | **Health Check Endpoints** | Cung cấp 3 endpoints kiểm tra trạng thái:<br>• `GET /health`: Tổng hợp toàn bộ (DB, Redis, MinIO).<br>• `GET /health/live`: Liveness probe (kiểm tra tiến trình còn sống).<br>• `GET /health/ready`: Readiness probe (kiểm tra kết nối PostgreSQL và Redis). | `Microsoft.Extensions.Diagnostics.HealthChecks`, `AspNetCore.HealthChecks.NpgSql`, `AspNetCore.HealthChecks.Redis`, `AspNetCore.HealthChecks.Minio`. |
| **FR-OBS-002** | **Structured Logging** | Tự động sinh `CorrelationId` (`X-Correlation-ID`) cho từng HTTP request. Ghi log toàn bộ thời gian xử lý qua MediatR `LoggingBehavior`. Tự động cảnh báo nếu request xử lý $> 500$ms. | Serilog sinks: Console (JSON), File (rolling daily), Seq (HTTP sink `:5341` trong development). |
| **FR-OBS-003** | **Distributed Tracing & Metrics** | Giám sát luồng phân tán xuyên suốt qua HTTP request, MediatR pipeline, EF Core queries. Đo lường metrics: request count, duration histogram, error rate. | OpenTelemetry .NET SDK, exporter OTLP đến Seq / Jaeger. |

---

# CHƯƠNG 4. YÊU CẦU PHI CHỨC NĂNG (NFR)

## 4.1. Hiệu năng (NFR-PERF)
- **NFR-PERF-001 (Response Time API):** p50 $\le 150$ms cho các request GET có cache. p95 $\le 500$ms cho toàn bộ API endpoints (kể cả ghi). p99 $\le 1000$ms trong mọi trường hợp tải bình thường.
- **NFR-PERF-002 (Throughput):** Hệ thống chịu tải đồng thời $\ge 100$ concurrent users trên hạ tầng tối thiểu (2 vCPU, 4GB RAM) mà không bị suy giảm hiệu năng.
- **NFR-PERF-003 (Cache Effectiveness):** Tỷ lệ Redis Cache Hit Rate $\ge 80\%$. TTL chuẩn: Danh mục (30–60 phút), Recipe Detail (5–15 phút), Search (1–5 phút). Invalidation theo sự kiện khi có Create/Update/Delete.
- **NFR-PERF-004 (Database Optimization):** Tuyệt đối không để xảy ra N+1 query (bắt buộc dùng `.Include()` / `.ThenInclude()` và projection DTO). Mọi cột trong mệnh đề `WHERE` và `ORDER BY` đều có index. Cảnh báo slow query nếu $> 100$ms.
- **NFR-PERF-005 (Frontend Core Web Vitals):** Đạt chuẩn Google Core Web Vitals: LCP $\le 2.5$s, CLS $\le 0.1$, INP $\le 200$ms. First Load JS Bundle $\le 200$KB (gzipped).

## 4.2. Bảo mật (NFR-SEC)
- **NFR-SEC-001 (Password Hashing):** Hash mật khẩu bằng PBKDF2-HMACSHA512 với số vòng lặp $\ge 100.000$ qua ASP.NET Core Identity.
- **NFR-SEC-002 (JWT Security):** Access Token dùng thuật toán HS256, TTL 15 phút. Refresh Token độ dài ngẫu nhiên 128-bit, băm SHA-256 khi lưu DB, TTL 7 ngày, bắt buộc Rotation & Token Reuse Detection.
- **NFR-SEC-003 (Rate Limiting):** Giới hạn tần suất request theo IP: Auth endpoints (`/auth/*`): 10 req/phút; API chung: 100 req/phút; File upload: 5 req/phút. Khi vượt quá trả về HTTP `429 Too Many Requests` kèm `Retry-After`.
- **NFR-SEC-004 (Input & File Upload Validation):** Kiểm tra chặt chẽ toàn bộ input tại tầng Application bằng FluentValidation; chống Path Traversal qua GUID filename; chống upload mã độc bằng việc kiểm tra MIME type kết hợp Magic Bytes.
- **NFR-SEC-005 (HTTPS & CORS):** Bắt buộc TLS 1.2+ trên production. Chính sách CORS chỉ cho phép các domain được cấu hình tường minh trong `appsettings` (không dùng wildcard `*`).

## 4.3. Khả năng Sử dụng (NFR-USE)
- **NFR-USE-001 (Responsive):** Giao diện tương thích hoàn hảo trên Mobile ($320\text{px} - 767\text{px}$), Tablet ($768\text{px} - 1199\text{px}$) và Desktop ($\ge 1200\text{px}$) bằng Tailwind CSS.
- **NFR-USE-002 (Accessibility - a11y):** Tuân thủ WCAG 2.1 Level AA (semantic HTML5 tags, nhãn ARIA, điều hướng bàn phím đầy đủ, tỷ lệ tương phản màu sắc $\ge 4.5:1$).
- **NFR-USE-003 (Error Feedback):** Thông báo lỗi rõ ràng theo format RFC 7807 ở backend, hiển thị inline validation lỗi trên từng input field ở frontend.
- **NFR-USE-004 (Loading UX):** Có visual feedback cho mọi tác vụ: Skeleton screen khi fetch data, optimistic update khi thao tác, toast message xác nhận, thanh tiến trình % khi upload ảnh.

## 4.4. Độ tin cậy (NFR-REL)
- **NFR-REL-001 (Uptime SLA):** Cam kết độ sẵn sàng hệ thống $\ge 99.5\%$.
- **NFR-REL-002 (Resilience):** Global Exception Middleware bắt toàn bộ lỗi chưa xử lý; tự động kết nối lại database connection pool; Redis failover graceful degradation (fallback xuống truy vấn DB trực tiếp nếu Redis gặp sự cố); Hangfire retry 3 lần cho background jobs.
- **NFR-REL-003 (Data Durability & Soft Delete):**
  - Đảm bảo toàn vẹn dữ liệu qua PostgreSQL WAL.
  - Refresh token lưu trong DB để không bị mất phiên khi restart hệ thống.
  - **Áp dụng Soft Delete:** Recipe và Category được đánh dấu `IsDeleted = true` thay vì xóa vật lý, cho phép bảo tồn dữ liệu và khôi phục khi cần thiết.

## 4.5. Khả năng Bảo trì (NFR-MAINT)
- **NFR-MAINT-001 (Code Quality):** Kiểm tra mã nguồn với SonarAnalyzer, EditorConfig cho .NET; ESLint và Prettier cho TypeScript/Next.js. Không có cảnh báo trình biên dịch khi build CI.
- **NFR-MAINT-002 (Test Coverage):** Độ phủ Unit Test $\ge 80\%$ line coverage cho Application commands, queries và validators. Đầy đủ integration tests cho toàn bộ REST endpoints.
- **NFR-MAINT-003 (Clean Architecture Compliance):** Tuân thủ nghiêm ngặt Dependency Rule: Tầng Domain tuyệt đối không phụ thuộc tầng ngoài (chỉ chứa entity và giao diện cốt lõi). Tầng Application chỉ phụ thuộc Domain.

## 4.6. Khả năng Mở rộng (NFR-SCALE)
- **NFR-SCALE-001 (Stateless Backend):** Backend hoàn toàn stateless: Xác thực qua JWT, dữ liệu cache dùng chung trên Redis (không dùng `IMemoryCache` cục bộ cho shared data), queue việc nền lưu trên PostgreSQL qua Hangfire, cho phép scale-out nhiều container backend đồng thời phía sau reverse proxy Nginx.
- **NFR-SCALE-002 (Database Scaling):** Cấu hình connection pooling với Npgsql. Áp dụng split query trong EF Core khi load nhiều collection liên kết. Đánh B-Tree và GIN index tối ưu.

## 4.7. Tối ưu SEO (NFR-SEO)
- **NFR-SEO-001 (Structured Data):** Tự động nhúng dữ liệu cấu trúc Schema.org Recipe (JSON-LD) cho từng trang công thức để hỗ trợ Google Rich Snippets.
- **NFR-SEO-002 (Meta Tags & Open Graph):** Tối ưu SEO title, meta description, thẻ Open Graph và Twitter Card cho việc chia sẻ mạng xã hội.
- **NFR-SEO-003 (Sitemap & Robots):** Tự động cập nhật `sitemap.xml` và cấu hình `robots.txt` cho phép tìm kiếm và lập chỉ mục.
- **NFR-SEO-004 (URL Structure):** URL thân thiện dạng `/recipes/{slug}` và `/categories/{slug}`. Không đổi slug sau khi công thức đã publish để tránh gãy liên kết.

---

# CHƯƠNG 5. YÊU CẦU GIAO DIỆN NGOÀI

## 5.1. Giao diện Người dùng (UI Next.js Routes)
| Tuyến đường (Route) | Mô tả chức năng | Cơ chế Rendering | Yêu cầu xác thực |
| :--- | :--- | :--- | :--- |
| `/` | Trang chủ: Recipe nổi bật, danh mục | ISR (`revalidate = 3600`) | Không |
| `/recipes` | Danh sách công thức (lọc, sắp xếp, tìm kiếm) | SSR (dynamic) | Không |
| `/recipes/[slug]` | Chi tiết công thức nấu ăn, dinh dưỡng, JSON-LD | ISR (`revalidate = 300`) | Không (Draft: Author/Admin) |
| `/categories` | Danh sách danh mục ẩm thực | ISR (`revalidate = 3600`) | Không |
| `/categories/[slug]`| Danh sách công thức theo danh mục | ISR (`revalidate = 600`) | Không |
| `/auth/login` | Màn hình đăng nhập (Email/Mật khẩu, Google) | CSR | Không (redirect nếu đã login) |
| `/auth/register` | Màn hình đăng ký tài khoản mới | CSR | Không |
| `/dashboard` | Trang tổng quan tác giả / admin | CSR | Bắt buộc (Author/Admin) |
| `/dashboard/recipes`| Quản lý công thức của tôi (Draft, Published) | CSR | Bắt buộc |
| `/dashboard/recipes/new` | Multi-step form tạo công thức mới | CSR | Bắt buộc |
| `/dashboard/recipes/[id]/edit` | Chỉnh sửa công thức nấu ăn | CSR | Bắt buộc (Owner/Admin) |
| `/dashboard/categories` | Quản trị danh mục ẩm thực | CSR | Bắt buộc (Admin) |
| `/profile` | Xem và cập nhật hồ sơ cá nhân | CSR | Bắt buộc |
| `/search` | Trang tìm kiếm toàn văn bản | SSR | Không |

## 5.2. Giao diện Phần mềm – REST API
- **Chuẩn giao tiếp:** RESTful API qua HTTP/1.1 và HTTP/2. Định dạng trao đổi dữ liệu: `application/json; charset=utf-8`.
- **Base URL Development:** `http://localhost:5000/api/v1`
- **Base URL Production:** `https://api.culinaryblog.com/api/v1`
- **Authentication:** `Authorization: Bearer <access_token>` trong HTTP Request Header. Refresh Token truyền trong request body.
- **Chuẩn hóa phản hồi lỗi (RFC 7807):**
  ```json
  {
    "type": "VALIDATION_ERROR",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "detail": "Please refer to the errors property for details.",
    "errors": {
      "title": ["Tiêu đề công thức phải có độ dài từ 5 đến 200 ký tự."]
    }
  }
  ```

## 5.3. Giao diện Dịch vụ Bên thứ ba
- **Google OAuth 2.0:** Xác thực tài khoản Google qua Authorization Code Flow kết hợp PKCE.
- **MinIO Object Storage:** Tương thích chuẩn AWS S3 API qua gói `AWSSDK.S3`.
- **Email Service:** Tích hợp gửi mail SMTP qua `MailKit` (Mailhog trong development).
- **Seq / OpenTelemetry:** Thu thập log và trace phân tán qua cổng HTTP/OTLP.

## 5.4. Giao diện Phần cứng
- Web application chuẩn, không yêu cầu thiết bị phần cứng đặc thù. Client chạy mượt mà trên mọi trình duyệt hiện đại hỗ trợ ES2020+ (Chrome, Firefox, Edge, Safari).

---

# CHƯƠNG 6. KIẾN TRÚC HỆ THỐNG

## 6.1. Tổng quan Kiến trúc
Hệ thống triển khai theo mô hình 2 tầng chính:
1. **Frontend:** Next.js 15 App Router với TypeScript, React Server Components (RSC) kết hợp Client Components, TanStack Query cho client-side state, Tailwind CSS cho giao diện.
2. **Backend:** ASP.NET Core .NET 10 Minimal APIs tuân thủ **Clean Architecture** và CQRS.

## 6.2. Kiến trúc Backend – Clean Architecture
Phân tầng dự án theo cấu trúc:
- **`CulinaryBlog.Domain` (Core):** Chứa Entities (`Recipe`, `Category`, `ApplicationUser`, `RecipeStep`, `RecipeIngredient`, `RecipeImage`), Owned Entity (`RecipeNutrition`), Enums (`RecipeDifficulty`, `RecipeStatus`), Domain Exceptions. Độc lập tuyệt đối, chỉ tham chiếu `Microsoft.Extensions.Identity.Stores`.
- **`CulinaryBlog.Application`:** Chứa Use Cases dạng Commands/Queries (CQRS), MediatR Handlers, DTOs, FluentValidation Validators, Pipeline Behaviors (`ValidationBehavior`, `LoggingBehavior`, `CachingBehavior`), Interfaces cho services (`IJwtService`, `IFileStorageService`, `IEmailService`, `ICurrentUser`).
- **`CulinaryBlog.Infrastructure`:** Chứa EF Core `CulinaryBlogDbContext`, cấu hình Entity Type Configurations, Database Migrations, cài đặt Repositories, MinIO S3 SDK client, Redis Cache implementation, Hangfire job configuration, MailKit client.
- **`CulinaryBlog.API` (Presentation):** Chứa các Minimal API Endpoint Groups (`AuthEndpoints`, `RecipesEndpoints`, `CategoriesEndpoints`), Middlewares (`GlobalExceptionMiddleware`, `CorrelationIdMiddleware`), Dependency Injection setup, Scalar/OpenAPI documentation.

## 6.3. CQRS + MediatR Pipeline
Mọi thao tác ghi (Command) và đọc (Query) đều đi qua MediatR Pipeline theo thứ tự:
1. **LoggingBehavior:** Ghi nhận request type, tham số đầu vào và đo lường thời gian thực thi (cảnh báo nếu $> 500$ms).
2. **ValidationBehavior:** Tự động quét và thực thi các rule FluentValidation tương ứng. Ném `ValidationException` (trả về HTTP 400) nếu dữ liệu không hợp lệ.
3. **CachingBehavior:** Đối với các Query implement interface `ICacheable`, tự động kiểm tra Redis Cache trước khi gọi database.
4. **Handler Execution:** Thực thi logic nghiệp vụ chính.
5. **CacheInvalidationBehavior:** Đối với các Command implement interface `ICacheInvalidator`, tự động xóa các cache key/tag liên quan sau khi thao tác ghi thành công.

## 6.4. Mô hình Quan hệ Thực thể (ERD tóm tắt)
- **`Recipe`**: Là Aggregate Root.
  - Quan hệ 1:N với `RecipeStep` (Cascade delete logic).
  - Quan hệ 1:N với `RecipeIngredient` (Cascade delete logic).
  - Quan hệ 1:N với `RecipeImage` (Bảng riêng biệt, không phải owned entity).
  - Quan hệ 1:1 với Owned Entity `RecipeNutrition` (nhúng trực tiếp các cột vào bảng `Recipes`).
  - Quan hệ N:1 với `Category` (Khóa ngoại `CategoryId`, restrict delete nếu danh mục còn công thức).
  - Quan hệ N:1 với `ApplicationUser` (Khóa ngoại `AuthorId`).
- **`ApplicationUser`**: Kế thừa `IdentityUser<string>`.
  - Quan hệ 1:N với `Recipe`.
  - Quan hệ 1:N với `RefreshToken`.

## 6.5. Triển khai – Docker Compose
Cấu hình chuẩn hóa các dịch vụ trong `docker-compose.yml`:
- `api`: .NET 10 Web API (Port `5000:8080`).
- `frontend`: Next.js Web App (Port `3000:3000`).
- `postgres`: PostgreSQL 16 Alpine (Port `5432:5432`, persistent volume `pgdata`).
- `redis`: Redis 7 Alpine (Port `6379:6379`, persistent AOF `redisdata`).
- `minio`: `quay.io/minio/minio:RELEASE.2024-01-16T16-07-38Z` (Port `9000:9000` API, `9001:9001` Console).
- `seq`: `datalust/seq:2023.4` (Port `5341:80`, development logging).
- `mailhog`: Mailhog SMTP/Web (Port `1025:1025` SMTP, `8025:8025` Web UI).

---

# CHƯƠNG 7. MÔ HÌNH DỮ LIỆU

Tất cả các bảng nghiệp vụ chính đều kế thừa từ lớp trừu tượng `BaseEntity` và áp dụng Soft Delete pattern.

## 7.1. BaseEntity (Abstract)
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid (Guid)` | PRIMARY KEY, DEFAULT `gen_random_uuid()` | Khóa chính UUID v4 chống brute-force ID. |
| `CreatedAt` | `timestamptz` | NOT NULL, DEFAULT `NOW()` | Thời điểm tạo bản ghi (set bởi Audit Interceptor). |
| `UpdatedAt` | `timestamptz` | NULL | Thời điểm cập nhật cuối cùng (set bởi Audit Interceptor). |
| `IsDeleted` | `boolean` | NOT NULL, DEFAULT `false` | Cờ xóa mềm. Tự động lọc qua Global Query Filter: `.Where(x => !x.IsDeleted)`. |
| `RowVersion`| `bytea (timestamp)` | NOT NULL, Concurrency Token | Quản lý xung đột cập nhật đồng thời (Optimistic Concurrency). |

## 7.2. Recipe
Bảng: `Recipes`
| Cột | Kiểu dữ liệu | Ràng buộc | Index | Mô tả |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK (kế thừa) | PK | Khóa chính. |
| `Title` | `varchar(200)` | NOT NULL | `IDX_Recipe_Title` | Tiêu đề công thức. |
| `Slug` | `varchar(220)` | NOT NULL, UNIQUE | `IDX_Recipe_Slug` (UNIQUE) | Định danh URL thân thiện SEO. Không đổi sau khi publish. |
| `Description` | `text` | NOT NULL | — | Mô tả ngắn gọn ($\le 2000$ ký tự), dùng cho card preview và meta tag. |
| `Instructions` | `text` | NOT NULL | — | Hướng dẫn tổng quan (legacy text/markdown). |
| `PrepTime` | `integer` | NOT NULL, CHECK $> 0$ | — | Thời gian chuẩn bị (phút). |
| `CookTime` | `integer` | NOT NULL, CHECK $\ge 0$ | — | Thời gian nấu (phút). |
| `Servings` | `integer` | NOT NULL, CHECK $> 0$ | — | Số khẩu phần ăn. |
| `Difficulty` | `smallint` | NOT NULL, DEFAULT `1` | `IDX_Recipe_Difficulty` | 1: Easy, 2: Medium, 3: Hard, 4: Expert. |
| `Status` | `smallint` | NOT NULL, DEFAULT `0` | `IDX_Recipe_Status` | 0: Draft, 1: Published, 2: Archived. |
| `CategoryId` | `uuid` | NOT NULL, FK $\rightarrow$ `Categories.Id` | `IDX_Recipe_CategoryId` | Khóa ngoại danh mục. ON DELETE RESTRICT. |
| `AuthorId` | `varchar(450)` | NOT NULL, FK $\rightarrow$ `AspNetUsers.Id`| `IDX_Recipe_AuthorId` | Khóa ngoại tác giả. |
| `SearchVector` | `tsvector` | NULL | `IDX_Recipe_Search` (GIN) | Vector tìm kiếm toàn văn bản tiếng Việt. |
| `PublishedAt` | `timestamptz` | NULL | `IDX_Recipe_PublishedAt` | Thời điểm xuất bản công thức. |
| `CreatedAt` | `timestamptz` | NOT NULL | — | (Kế thừa từ BaseEntity) |
| `UpdatedAt` | `timestamptz` | NULL | — | (Kế thừa từ BaseEntity) |
| `IsDeleted` | `boolean` | NOT NULL, DEFAULT `false` | `IDX_Recipe_IsDeleted` (partial) | Đánh dấu xóa mềm. |
| `RowVersion` | `bytea` | NOT NULL | — | Concurrency Token. |

### 7.2.1. RecipeNutrition (Owned Entity — Cột trong bảng Recipes)
Các cột dinh dưỡng được lưu trực tiếp vào bảng `Recipes` với tiền tố `Nutrition_`:
| Cột trong DB | Thuộc tính C# | Kiểu dữ liệu | Mô tả |
| :--- | :--- | :--- | :--- |
| `Nutrition_Calories` | `Calories` | `decimal(8,2)?` | Năng lượng (kcal / khẩu phần). Nullable. |
| `Nutrition_Protein` | `Protein` | `decimal(8,2)?` | Chất đạm (gram). Nullable. |
| `Nutrition_Carbohydrates` | `Carbohydrates` | `decimal(8,2)?` | Tinh bột (gram). Nullable. |
| `Nutrition_Fat` | `Fat` | `decimal(8,2)?` | Chất béo (gram). Nullable. |
| `Nutrition_Fiber` | `Fiber` | `decimal(8,2)?` | Chất xơ (gram). Nullable. |
| `Nutrition_Sodium` | `Sodium` | `decimal(8,2)?` | Natri (mg). Nullable. |

## 7.3. RecipeStep
Bảng: `RecipeSteps`
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK (BaseEntity) | Khóa chính. |
| `RecipeId` | `uuid` | NOT NULL, FK $\rightarrow$ `Recipes.Id` | Khóa ngoại Recipe. Cascade delete khi recipe bị xóa. |
| `StepNumber` | `integer` | NOT NULL, CHECK $> 0$ | Thứ tự bước ($1, 2, 3...$). Tự động sinh hoặc renumber. |
| `Title` | `varchar(200)` | NOT NULL | Tên ngắn gọn của bước (ví dụ: "Sơ chế nguyên liệu"). |
| `Description` | `text` | NOT NULL | Nội dung chi tiết các thao tác thực hiện. |
| `TimerMinutes` | `integer` | NULL, CHECK $\ge 0$ | Thời gian hẹn giờ cho bước (phút). Nullable. |
| `ImageUrl` | `varchar(500)` | NULL | Ảnh minh họa riêng cho bước (trên MinIO). Nullable. |

## 7.4. RecipeIngredient
Bảng: `RecipeIngredients`
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK (BaseEntity) | Khóa chính. |
| `RecipeId` | `uuid` | NOT NULL, FK $\rightarrow$ `Recipes.Id` | Khóa ngoại Recipe. Cascade delete khi recipe bị xóa. |
| `Name` | `varchar(200)` | NOT NULL | Tên nguyên liệu (ví dụ: "Thịt thăn bò"). |
| `Quantity` | `decimal(10,3)` | **NULL** | Số lượng định lượng. **Nullable** cho nguyên liệu "vừa đủ". |
| `Unit` | `varchar(50)` | **NULL** | Đơn vị đo lường (gram, ml, muỗng...). **Nullable**. |
| `Notes` | `varchar(500)` | NULL | Ghi chú thêm (ví dụ: "thái lát mỏng", "nêm vừa ăn"). |
| `OrderIndex` | `integer` | NOT NULL, DEFAULT `0` | Thứ tự hiển thị trong danh sách nguyên liệu. |

## 7.5. RecipeImage
Bảng: `RecipeImages` (Bảng riêng biệt quan hệ 1:N với Recipes)
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK (BaseEntity) | Khóa chính ảnh. |
| `RecipeId` | `uuid` | NOT NULL, FK $\rightarrow$ `Recipes.Id` | Khóa ngoại tham chiếu công thức nấu ăn. |
| `OriginalUrl` | `varchar(500)` | NOT NULL | Đường dẫn ảnh gốc trên MinIO. |
| `MediumUrl` | `varchar(500)` | NULL | Đường dẫn ảnh kích thước vừa ($800 \times 600$). |
| `ThumbnailUrl`| `varchar(500)` | NULL | Đường dẫn ảnh thumbnail ($300 \times 300$). |
| `AltText` | `varchar(200)` | NULL | Văn bản thay thế mô tả ảnh cho người khiếm thị / SEO. |
| `IsPrimary` | `boolean` | NOT NULL, DEFAULT `false` | Đánh dấu ảnh đại diện chính của công thức. |
| `OrderIndex` | `integer` | NOT NULL, DEFAULT `0` | Thứ tự hiển thị trong bộ sưu tập gallery. |

## 7.6. Category
Bảng: `Categories`
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK (BaseEntity) | Khóa chính danh mục. |
| `Name` | `varchar(100)` | NOT NULL, UNIQUE | Tên danh mục ẩm thực (ví dụ: "Món khai vị"). |
| `Slug` | `varchar(120)` | NOT NULL, UNIQUE | URL slug chuẩn SEO. |
| `Description` | `text` | NULL | Mô tả danh mục. |
| `ImageUrl` | `varchar(500)` | NULL | Ảnh đại diện danh mục. |
| `OrderIndex` | `integer` | NOT NULL, DEFAULT `0` | Thứ tự hiển thị trên thanh điều hướng. |
| `IsDeleted` | `boolean` | NOT NULL, DEFAULT `false` | Cờ xóa mềm cho danh mục. |

## 7.7. ApplicationUser (extends IdentityUser)
Bảng: `AspNetUsers` (Bổ sung các cột tùy biến)
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `DisplayName` | `varchar(100)` | NOT NULL | Tên hiển thị công khai của tác giả (không phải username). |
| `AvatarUrl` | `varchar(500)` | NULL | Ảnh đại diện (avatar). |
| `Bio` | `text` | NULL | Tiểu sử tóm tắt của tác giả. |
| `IsActive` | `boolean` | NOT NULL, DEFAULT `true` | Trạng thái tài khoản (Admin có thể khóa). |
| `CreatedAt` | `timestamptz` | NOT NULL, DEFAULT `NOW()` | Ngày giờ khởi tạo tài khoản. |

## 7.8. RefreshToken
Bảng: `RefreshTokens`
| Cột | Kiểu dữ liệu | Ràng buộc | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PRIMARY KEY | Khóa chính token. |
| `UserId` | `varchar(450)` | NOT NULL, FK $\rightarrow$ `AspNetUsers.Id` | Người sở hữu token. |
| `TokenHash` | `varchar(64)` | NOT NULL, UNIQUE | Chuỗi băm SHA-256 của raw refresh token. |
| `ExpiresAt` | `timestamptz` | NOT NULL | Thời hạn hết hiệu lực (7 ngày kể từ khi cấp). |
| `RevokedAt` | `timestamptz` | NULL | Thời điểm bị thu hồi (NULL = còn hiệu lực). |
| `ReplacedByTokenHash` | `varchar(64)` | NULL | Lưu hash của token mới cấp để phục vụ Token Family Audit. |
| `CreatedAt` | `timestamptz` | NOT NULL, DEFAULT `NOW()` | Thời điểm cấp token. |
| `CreatedByIp` | `varchar(45)` | NULL | Địa chỉ IP của client yêu cầu cấp token. |

---

# CHƯƠNG 8. ĐẶC TẢ REST API

## 8.1. Authentication Module (/auth)
| Method | Endpoint | Yêu cầu Auth | Request Body / Query Params | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/auth/register` | Public | `{ email, password, displayName, userName? }` | Đăng ký tác giả mới.<br>• `201 Created`: `{ accessToken, refreshToken, expiresIn, user }`<br>• `400 Bad Request`: Validation errors<br>• `409 Conflict`: Email đã tồn tại |
| `POST` | `/api/v1/auth/login` | Public | `{ email, password }` | Đăng nhập tài khoản.<br>• `200 OK`: `{ accessToken, refreshToken, expiresIn, user }`<br>• `401 Unauthorized`: Sai thông tin đăng nhập<br>• `423 Locked`: Bị khóa do thử sai nhiều lần<br>• `429 Too Many Requests`: Vượt rate limit |
| `POST` | `/api/v1/auth/google` | Public | `{ idToken }` | Đăng nhập qua Google OAuth 2.0.<br>• `200 OK`: Trả về cặp token và user info<br>• `400 Bad Request`: ID token không hợp lệ |
| `POST` | `/api/v1/auth/refresh` | Public | `{ refreshToken }` | Làm mới Access Token (Rotation).<br>• `200 OK`: Cặp token mới `{ accessToken, refreshToken }`<br>• `401 Unauthorized`: Token hết hạn hoặc bị thu hồi |
| `POST` | `/api/v1/auth/logout` | Bearer JWT | `{ refreshToken }` | Đăng xuất và thu hồi Refresh Token.<br>• `204 No Content`: Thành công |
| `GET` | `/api/v1/auth/me` | Bearer JWT | — | Lấy thông tin hồ sơ người dùng hiện tại.<br>• `200 OK`: `{ id, email, userName, displayName, avatarUrl, bio, roles }` |
| `PATCH`| `/api/v1/auth/me` | Bearer JWT | `{ displayName?, avatarUrl?, bio? }` | Cập nhật hồ sơ cá nhân.<br>• `200 OK`: Thông tin hồ sơ mới sau cập nhật<br>• `400 Bad Request`: Dữ liệu không hợp lệ |

## 8.2. Categories Module (/categories)
| Method | Endpoint | Yêu cầu Auth | Request Body / Query Params | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/categories` | Public | — | Danh sách tất cả danh mục kèm số lượng recipes.<br>• `200 OK`: `CategoryDto[]` |
| `GET` | `/api/v1/categories/{slug}`| Public | `?page=1&pageSize=12&sortBy=createdAt&sortOrder=desc` | Chi tiết danh mục và danh sách công thức thuộc danh mục.<br>• `200 OK`: `{ category, recipes: PagedResult }`<br>• `404 Not Found`: Danh mục không tồn tại |
| `POST` | `/api/v1/categories` | Admin | `{ name, description?, imageUrl?, orderIndex? }` | Tạo danh mục ẩm thực mới.<br>• `201 Created`: `CategoryDto`<br>• `400 Bad Request`: Lỗi validation<br>• `403 Forbidden`: Không phải Admin<br>• `409 Conflict`: Tên danh mục đã tồn tại |
| `PUT` | `/api/v1/categories/{id}`| Admin | `{ name, description?, imageUrl?, orderIndex? }` | Cập nhật danh mục (giữ nguyên slug).<br>• `200 OK`: `CategoryDto`<br>• `404 Not Found`: Không tìm thấy ID |
| `DELETE`| `/api/v1/categories/{id}`| Admin | — | Xóa mềm danh mục (`IsDeleted = true`).<br>• `204 No Content`: Xóa thành công<br>• `409 Conflict`: Còn recipes thuộc về danh mục này |

## 8.3. Recipes Module (/recipes)
| Method | Endpoint | Yêu cầu Auth | Request Body / Query Params | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/recipes` | Public | `?page=1&pageSize=12&categoryId={guid}&difficulty={level}&maxCookTime={min}&sortBy={field}&sortOrder={asc\|desc}` | Danh sách công thức có phân trang, lọc và sắp xếp.<br>• `200 OK`: `PagedResult<RecipeSummaryDto>`<br>• `400 Bad Request`: Tham số query không hợp lệ |
| `GET` | `/api/v1/recipes/{slug}` | Public / Author | — | Chi tiết công thức nấu ăn.<br>• `200 OK`: `RecipeDetailDto`<br>• `403 Forbidden`: Recipe Draft mà không phải tác giả sở hữu<br>• `404 Not Found`: Slug không tồn tại |
| `GET` | `/api/v1/recipes/search` | Public | `?q={keyword}&page=1&pageSize=10` | Tìm kiếm toàn văn bản tiếng Việt không dấu.<br>• `200 OK`: `PagedResult<RecipeSummaryDto>` |
| `POST` | `/api/v1/recipes` | Author / Admin | `{ title, description, categoryId, prepTimeMinutes, cookTimeMinutes, servings, difficulty, instructions, nutrition?, ingredients?, steps? }` | Tạo công thức mới (mặc định trạng thái Draft).<br>• `201 Created`: `RecipeDto`<br>• `400 Bad Request`: Lỗi dữ liệu<br>• `409 Conflict`: Trùng slug |
| `PUT` | `/api/v1/recipes/{id}` | Owner / Admin | `{ title, description, categoryId, prepTimeMinutes, cookTimeMinutes, servings, difficulty, instructions, rowVersion }` | Cập nhật thông tin công thức (kiểm tra `RowVersion`).<br>• `200 OK`: `RecipeDto`<br>• `403 Forbidden`: Không phải chủ sở hữu<br>• `409 Conflict`: Xung đột phiên bản (`RowVersion` mismatch) |
| `PATCH`| `/api/v1/recipes/{id}/publish` | Owner / Admin | — | Xuất bản công thức.<br>• `200 OK`: `RecipeDto`<br>• `400 Bad Request`: Thiếu nguyên liệu hoặc bước thực hiện (`RECIPE_PUBLISH_INCOMPLETE`) |
| `PATCH`| `/api/v1/recipes/{id}/unpublish` | Owner / Admin | — | Hủy xuất bản (chuyển về Draft).<br>• `200 OK`: `RecipeDto` |
| `PATCH`| `/api/v1/recipes/{id}/archive` | Owner / Admin | — | Lưu trữ / Ẩn công thức.<br>• `200 OK`: `RecipeDto` |
| `DELETE`| `/api/v1/recipes/{id}` | Owner / Admin | — | **Xóa mềm** công thức (`IsDeleted = true`).<br>• `204 No Content`: Xóa thành công<br>• `403 Forbidden`: Không có quyền |

## 8.4. Recipe Images (/recipes/{id}/images)
| Method | Endpoint | Yêu cầu Auth | Request / Body | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/recipes/{id}/images` | Owner / Admin | `multipart/form-data`: `file`, `altText?` | Tải ảnh lên MinIO và liên kết vào Recipe.<br>• `201 Created`: `{ id, originalUrl, isPrimary }`<br>• `400 Bad Request`: Quá 5MB hoặc sai định dạng MIME/Magic Bytes |
| `PATCH`| `/api/v1/recipes/{id}/images/{imageId}/primary` | Owner / Admin | — | Đặt ảnh làm ảnh đại diện chính.<br>• `200 OK`: Image updated |
| `DELETE`| `/api/v1/recipes/{id}/images/{imageId}` | Owner / Admin | — | Xóa ảnh khỏi Recipe và xóa tệp trên MinIO.<br>• `204 No Content`: Xóa thành công |

## 8.5. Recipe Steps (/recipes/{id}/steps)
| Method | Endpoint | Yêu cầu Auth | Request Body | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/recipes/{id}/steps` | Owner / Admin | `{ stepNumber?, title, description, timerMinutes?, imageUrl? }` | Thêm bước mới (tự động tăng `stepNumber` nếu rỗng).<br>• `201 Created`: `RecipeStepDto`<br>• `400 Bad Request`: Thiếu `title` hoặc `description` |
| `PUT` | `/api/v1/recipes/{id}/steps/{stepId}` | Owner / Admin | `{ stepNumber?, title?, description?, timerMinutes?, imageUrl? }` | Cập nhật bước nấu ăn.<br>• `200 OK`: `RecipeStepDto` |
| `DELETE`| `/api/v1/recipes/{id}/steps/{stepId}` | Owner / Admin | — | Xóa bước nấu ăn và tự động renumber các bước còn lại.<br>• `204 No Content`: Xóa thành công |

## 8.6. Recipe Ingredients (/recipes/{id}/ingredients)
| Method | Endpoint | Yêu cầu Auth | Request Body | Mô tả & Response Codes |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/recipes/{id}/ingredients` | Owner / Admin | `{ name, quantity?, unit?, notes?, orderIndex? }` | Thêm nguyên liệu (quantity/unit cho phép null).<br>• `201 Created`: `RecipeIngredientDto` |
| `PUT` | `/api/v1/recipes/{id}/ingredients/{ingId}` | Owner / Admin | `{ name?, quantity?, unit?, notes?, orderIndex? }` | Cập nhật nguyên liệu.<br>• `200 OK`: `RecipeIngredientDto` |
| `DELETE`| `/api/v1/recipes/{id}/ingredients/{ingId}` | Owner / Admin | — | Xóa nguyên liệu.<br>• `204 No Content`: Xóa thành công |

## 8.7. Health Check Endpoints
| Method | Endpoint | Yêu cầu Auth | Mô tả | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/health` | Public | Kiểm tra tổng hợp toàn bộ dependencies | `200 OK` (Healthy) hoặc `503 Service Unavailable` |
| `GET` | `/health/live` | Public | Liveness probe: Kiểm tra tiến trình backend | `200 OK` (Healthy) |
| `GET` | `/health/ready`| Public | Readiness probe: Kiểm tra kết nối DB và Redis | `200 OK` (Sẵn sàng nhận request) |

---

# PHỤ LỤC

## Phụ lục A – HTTP Status Codes
| Code | Trạng thái HTTP | Ngữ cảnh áp dụng trong hệ thống |
| :--- | :--- | :--- |
| **`200`** | **OK** | GET thành công; PUT/PATCH cập nhật thành công; POST login/refresh token thành công. |
| **`201`** | **Created** | Tạo mới thành công (User, Recipe, Category, Step, Ingredient, Image). Trả kèm entity vừa tạo. |
| **`204`** | **No Content** | Xóa thành công (Delete Recipe, Step, Ingredient, Image); Logout thành công. |
| **`400`** | **Bad Request** | Dữ liệu đầu vào sai validation (FluentValidation); upload file sai MIME/magic bytes/vượt 5MB; vi phạm điều kiện publish recipe (`RECIPE_PUBLISH_INCOMPLETE`). |
| **`401`** | **Unauthorized** | Thiếu hoặc token không hợp lệ; Refresh token hết hạn hoặc đã bị thu hồi. |
| **`403`** | **Forbidden** | Đã đăng nhập nhưng không có quyền thao tác (Author cố sửa recipe của người khác, tài khoản bị khóa). |
| **`404`** | **Not Found** | Tài nguyên không tồn tại hoặc đã bị xóa mềm (`IsDeleted = true`). |
| **`409`** | **Conflict** | Trùng lặp dữ liệu duy nhất (email, slug danh mục/công thức); xóa danh mục đang chứa recipes; xung đột cập nhật đồng thời (`RowVersion` mismatch). |
| **`423`** | **Locked** | Tài khoản bị tạm khóa sau 5 lần đăng nhập thất bại. |
| **`429`** | **Too Many Requests**| Vượt quá giới hạn Rate Limiting. Trả kèm header `Retry-After`. |
| **`500`** | **Internal Server Error** | Lỗi ngoại lệ hệ thống chưa được kiểm soát. Format lỗi RFC 7807, không để lộ stack trace. |
| **`503`** | **Service Unavailable** | Lỗi khi Health Check phát hiện DB hoặc Redis bị mất kết nối. |

## Phụ lục B – Application Error Codes
Chuỗi định danh lỗi nghiệp vụ được trả trong trường `type` hoặc `title` của RFC 7807:
| Error Code | HTTP Status | Mô tả chi tiết | Module |
| :--- | :--- | :--- | :--- |
| `AUTH_EMAIL_EXISTS` | 409 | Email đã được đăng ký bởi tài khoản khác trong hệ thống. | Auth |
| `AUTH_INVALID_CREDENTIALS` | 401 | Email hoặc mật khẩu không chính xác. | Auth |
| `AUTH_TOKEN_EXPIRED` | 401 | JWT Access Token đã hết hạn. | Auth |
| `AUTH_TOKEN_INVALID` | 401 | Access Token sai định dạng hoặc chữ ký không hợp lệ. | Auth |
| `AUTH_REFRESH_TOKEN_EXPIRED` | 401 | Refresh Token đã quá hạn 7 ngày. | Auth |
| `AUTH_REFRESH_TOKEN_REVOKED` | 401 | Refresh Token đã bị thu hồi (phát hiện Token Reuse Attack). | Auth |
| `AUTH_GOOGLE_TOKEN_INVALID` | 400 | Google ID Token không hợp lệ hoặc đã hết hạn. | Auth |
| `AUTH_ACCOUNT_DISABLED` | 403 | Tài khoản đã bị quản trị viên vô hiệu hóa (`IsActive = false`). | Auth |
| `RECIPE_NOT_FOUND` | 404 | Không tìm thấy công thức hoặc công thức đã bị xóa mềm. | Recipe |
| `RECIPE_SLUG_EXISTS` | 409 | Slug của công thức đã tồn tại. | Recipe |
| `RECIPE_PUBLISH_INCOMPLETE` | 400 | Không đủ điều kiện xuất bản: bắt buộc có ít nhất 1 nguyên liệu và 1 bước thực hiện. | Recipe |
| `RECIPE_FORBIDDEN` | 403 | Người dùng không phải chủ sở hữu của công thức và không phải Admin. | Recipe |
| `RECIPE_CONCURRENCY_CONFLICT` | 409 | Dữ liệu đã bị thay đổi bởi phiên làm việc khác (`RowVersion` không khớp). | Recipe |
| `CATEGORY_NOT_FOUND` | 404 | Danh mục ẩm thực không tồn tại. | Category |
| `CATEGORY_NAME_EXISTS` | 409 | Tên danh mục ẩm thực đã tồn tại. | Category |
| `CATEGORY_DELETE_HAS_RECIPES` | 409 | Không thể xóa danh mục khi còn công thức trực thuộc. | Category |
| `FILE_SIZE_EXCEEDED` | 400 | Dung lượng tệp tải lên vượt quá giới hạn cho phép (5 MB). | File |
| `FILE_MIME_INVALID` | 400 | Định dạng tệp không được hỗ trợ hoặc magic bytes không khớp. | File |
| `VALIDATION_ERROR` | 400 | Lỗi tính hợp lệ của dữ liệu đầu vào. Chi tiết xem trong `errors`. | Common |
| `RATE_LIMIT_EXCEEDED` | 429 | Gửi yêu cầu quá tần suất cho phép. Xem header `Retry-After`. | Common |

## Phụ lục C – Từ điển Thuật ngữ
- **Access Token (AT):** JSON Web Token (JWT) định danh người dùng trong các request API, thời hạn 15 phút.
- **Refresh Token (RT):** Chuỗi token ngẫu nhiên bảo mật lưu tại database dùng để cấp mới Access Token, thời hạn 7 ngày.
- **Refresh Token Rotation:** Cơ chế cấp mới Refresh Token và hủy bỏ token cũ trong mỗi lần làm mới, giúp ngăn ngừa tấn công chiếm quyền phiên.
- **Token Reuse Detection:** Phát hiện khi một refresh token đã bị hủy được đem ra tái sử dụng, từ đó tự động vô hiệu hóa toàn bộ các token liên quan (Token Family).
- **Clean Architecture:** Kiến trúc phần mềm tách biệt các mối quan tâm thành các tầng độc lập; phụ thuộc chỉ hướng vào trong tầng Domain.
- **CQRS (Command Query Responsibility Segregation):** Mẫu kiến trúc tách biệt luồng ghi (Command) và luồng đọc dữ liệu (Query).
- **Global Query Filter:** Cơ chế của EF Core tự động chèn thêm điều kiện lọc (ví dụ: `!IsDeleted`) vào mọi truy vấn database.
- **Optimistic Concurrency Control:** Cơ chế kiểm soát xung đột dữ liệu bằng `RowVersion` mà không cần lock database, phát hiện conflict khi lưu.
- **Incremental Static Regeneration (ISR):** Tính năng của Next.js cho phép cập nhật các trang web tĩnh trong nền theo chu kỳ định sẵn mà không cần build lại toàn bộ ứng dụng.
- **Full-Text Search (FTS):** Kỹ thuật tìm kiếm toàn văn bản nâng cao bằng chỉ mục đảo (GIN Index) và biểu thức `tsquery` trong PostgreSQL.
- **Soft Delete:** Kỹ thuật đánh dấu bản ghi đã xóa bằng cờ logic `IsDeleted = true` để bảo toàn dữ liệu và toàn vẹn tham chiếu.
