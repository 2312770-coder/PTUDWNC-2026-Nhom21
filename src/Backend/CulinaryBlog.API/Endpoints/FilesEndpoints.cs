using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryBlog.API.Endpoints;

public static class FilesEndpoints
{
    public static IEndpointRouteBuilder MapFilesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/files").WithTags("Files");

        // FR-FILE-001: Upload file ảnh lên MinIO
        group.MapPost("/upload", async (
            IFormFile file,
            string? folder,
            IFileStorageService fileStorage,
            CancellationToken ct) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.Problem(
                    title: "File không hợp lệ",
                    detail: "Vui lòng chọn một tệp tin hợp lệ.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            await using var stream = file.OpenReadStream();
            var targetFolder = string.IsNullOrWhiteSpace(folder) ? "general" : folder;
            var url = await fileStorage.UploadAsync(stream, file.FileName, file.ContentType, targetFolder, ct);

            return Results.Ok(ApiResponse.Ok(new
            {
                url,
                fileName = file.FileName,
                contentType = file.ContentType,
                size = file.Length
            }));
        })
        .WithName("UploadFile")
        .WithSummary("Upload file ảnh trực tiếp lên MinIO (FR-FILE-001)")
        .DisableAntiforgery();

        // FR-FILE-002: Xóa file khỏi MinIO
        group.MapDelete("/", async (
            [FromQuery] string fileUrl,
            IFileStorageService fileStorage,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Results.Problem(
                    title: "Tham số thiếu",
                    detail: "Vui lòng cung cấp URL file cần xóa.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            await fileStorage.DeleteAsync(fileUrl, ct);
            return Results.NoContent();
        })
        .WithName("DeleteFile")
        .WithSummary("Xóa file khỏi MinIO (FR-FILE-002)");

        return app;
    }
}
