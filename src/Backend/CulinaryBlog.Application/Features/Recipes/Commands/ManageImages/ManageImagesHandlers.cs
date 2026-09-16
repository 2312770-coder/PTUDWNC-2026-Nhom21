using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageImages;

// FR-RCP-008 - CHƯA HIỆN THỰC. Phần này phối hợp với FR-FILE (upload MinIO)
// và FR-JOB-002 (resize ảnh nền).
//
// Gợi ý upload:
//   1. Kiểm tra recipe tồn tại + quyền sở hữu.
//   2. Validate file: size <= 5MB, ContentType nằm trong danh sách cho phép,
//      và đọc vài byte đầu để kiểm tra magic bytes (JPEG: FF D8 FF,
//      PNG: 89 50 4E 47...). Sai -> ValidationException (400).
//   3. Gọi _fileStorage.UploadAsync(..., folder: $"recipes/{recipeId}") -> URL.
//   4. Tạo RecipeImage.Create(...), nếu IsPrimary = true thì phải bỏ cờ primary
//      của các ảnh còn lại (chỉ 1 ảnh primary / recipe).
//   5. Đẩy job resize (FR-JOB-002) dạng fire-and-forget, không chờ.
public class UploadRecipeImageCommandHandler : IRequestHandler<UploadRecipeImageCommand, RecipeImageDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUser _currentUser;

    public UploadRecipeImageCommandHandler(IRecipeRepository recipeRepository,
        IFileStorageService fileStorage, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    public Task<RecipeImageDto> Handle(UploadRecipeImageCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-008 (Upload ảnh công thức) chưa được hiện thực.");
}

// Gợi ý: gọi image.UpdateMetadata(...). Nếu đặt IsPrimary = true thì nhớ bỏ cờ
// primary của các ảnh khác trong cùng recipe.
public class UpdateRecipeImageCommandHandler : IRequestHandler<UpdateRecipeImageCommand, RecipeImageDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateRecipeImageCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeImageDto> Handle(UpdateRecipeImageCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-008 (Cập nhật metadata ảnh) chưa được hiện thực.");
}

// Gợi ý: xóa bản ghi trong DB trước, file trên MinIO xóa bất đồng bộ qua
// Hangfire (FR-FILE-002) để không làm chậm response.
public class DeleteRecipeImageCommandHandler : IRequestHandler<DeleteRecipeImageCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteRecipeImageCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(DeleteRecipeImageCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-008 (Xóa ảnh công thức) chưa được hiện thực.");
}
