using CulinaryBlog.API;
using CulinaryBlog.API.Endpoints;
using CulinaryBlog.API.Middleware;
using CulinaryBlog.Application;
using CulinaryBlog.Infrastructure;
using CulinaryBlog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Đăng ký service theo từng tầng (SRS mục 6.2) ──────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

// FR-OBS-001: health check. Tag "ready" dành cho readiness probe.
// TODO (người phụ trách FR-OBS-001): bổ sung check cho Redis và MinIO.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CulinaryBlogDbContext>(name: "database", tags: new[] { "ready" });

var app = builder.Build();

// ── Chỉ chạy ở Development: áp dụng migration + tạo sẵn 2 role ────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Culinary Blog API"));

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CulinaryBlogDbContext>();
    await db.Database.MigrateAsync();

    await DatabaseSeeder.SeedAsync(app.Services);
}


// ── Middleware pipeline - THỨ TỰ RẤT QUAN TRỌNG, đừng đảo lộn ─────────────
app.UseExceptionHandler();

// Gắn CorrelationId sớm nhất có thể để mọi log phía sau đều có mã này (FR-OBS-002).
app.UseMiddleware<CorrelationIdMiddleware>();

// Dev chạy HTTP cổng 5000 (SRS mục 5.2). Production mới dùng HTTPS và do
// Nginx đảm nhiệm SSL termination (SRS mục 6.5).
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("NextJsPolicy");   // CORS phải đứng trước Authentication
app.UseRateLimiter();
app.UseAuthentication();       // xác thực trước
app.UseAuthorization();        // rồi mới phân quyền

// ── Endpoint của từng module ──────────────────────────────────────────────
app.MapAuthEndpoints();        // FR-AUTH (SRS mục 8.1)
app.MapCategoriesEndpoints();  // FR-CAT  (SRS mục 8.2)
app.MapRecipesEndpoints();     // FR-RCP + FR-SRCH (SRS mục 8.3 - 8.6)
app.MapHealthEndpoints();      // FR-OBS-001 (SRS mục 8.7)
app.MapFilesEndpoints();       // FR-FILE-001 + FR-FILE-002 (Upload/Delete MinIO)

// TODO (người phụ trách FR-JOB): map dashboard Hangfire tại /hangfire,
// chỉ cho role Admin truy cập. Xem Infrastructure/Jobs/README_JOBS.md.

app.Run();
