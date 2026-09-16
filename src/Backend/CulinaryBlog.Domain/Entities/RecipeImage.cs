using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.5 - ảnh công thức lưu trên MinIO.
// MediumUrl/ThumbnailUrl do FR-JOB-002 (Hangfire resize) sinh sau, nên nullable.
// Chỉ có duy nhất 1 ảnh IsPrimary=true trên mỗi Recipe.
public class RecipeImage : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public string OriginalUrl { get; private set; } = string.Empty;
    public string? MediumUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? AltText { get; private set; }
    public bool IsPrimary { get; private set; }
    public int OrderIndex { get; private set; }

    public Recipe? Recipe { get; private set; }

    protected RecipeImage() { }

    public static RecipeImage Create(Guid recipeId, string originalUrl,
        string? altText = null, bool isPrimary = false, int orderIndex = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalUrl, nameof(originalUrl));
        return new RecipeImage
        {
            RecipeId = recipeId,
            OriginalUrl = originalUrl,
            AltText = altText?.Trim(),
            IsPrimary = isPrimary,
            OrderIndex = orderIndex,
        };
    }

    // Gọi từ background job FR-JOB-002 sau khi resize xong.
    public void SetGeneratedUrls(string? mediumUrl, string? thumbnailUrl)
    {
        MediumUrl = mediumUrl;
        ThumbnailUrl = thumbnailUrl;
        Touch();
    }

    public void UpdateMetadata(string? altText, bool? isPrimary, int? orderIndex)
    {
        AltText = altText ?? AltText;
        if (isPrimary.HasValue) IsPrimary = isPrimary.Value;
        if (orderIndex.HasValue) OrderIndex = orderIndex.Value;
        Touch();
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        Touch();
    }
}
