using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.3 - các bước thực hiện, sắp xếp theo StepNumber.
// StepNumber là composite unique cùng RecipeId.
public class RecipeStep : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public int StepNumber { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int? TimerMinutes { get; private set; }
    public string? ImageUrl { get; private set; }

    public Recipe? Recipe { get; private set; }

    protected RecipeStep() { }

    public static RecipeStep Create(Guid recipeId, int stepNumber, string title,
        string description, int? timerMinutes = null, string? imageUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));
        if (stepNumber <= 0)
            throw new DomainException("StepNumber phải lớn hơn 0.");

        return new RecipeStep
        {
            RecipeId = recipeId,
            StepNumber = stepNumber,
            Title = title.Trim(),
            Description = description.Trim(),
            TimerMinutes = timerMinutes,
            ImageUrl = imageUrl,
        };
    }

    public void Update(int? stepNumber, string? title, string? description,
        int? timerMinutes, string? imageUrl)
    {
        if (stepNumber.HasValue) StepNumber = stepNumber.Value;
        if (!string.IsNullOrWhiteSpace(title)) Title = title.Trim();
        if (!string.IsNullOrWhiteSpace(description)) Description = description.Trim();
        TimerMinutes = timerMinutes;
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        Touch();
    }

    // Gỡ bỏ ảnh minh họa của bước nấu (khi người dùng xóa ảnh)
    public void RemoveImage()
    {
        ImageUrl = null;
        Touch();
    }

    // Gọi khi xóa/chèn bước ở giữa, cần đánh số lại các bước còn lại (FR-RCP-010).
    public void Renumber(int newStepNumber)
    {
        StepNumber = newStepNumber;
        Touch();
    }
}
