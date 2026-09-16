using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.4 - nguyên liệu của công thức.
// Quantity và Unit nullable vì có loại "nguyên liệu vừa đủ" không cần định lượng.
public class RecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal? Quantity { get; private set; }
    public string? Unit { get; private set; }
    public string? Notes { get; private set; }
    public int OrderIndex { get; private set; }

    public Recipe? Recipe { get; private set; }

    protected RecipeIngredient() { }

    public static RecipeIngredient Create(Guid recipeId, string name,
        decimal? quantity = null, string? unit = null, string? notes = null, int orderIndex = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        return new RecipeIngredient
        {
            RecipeId = recipeId,
            Name = name.Trim(),
            Quantity = quantity,
            Unit = unit?.Trim(),
            Notes = notes?.Trim(),
            OrderIndex = orderIndex,
        };
    }

    public void Update(string? name, decimal? quantity, string? unit, string? notes, int? orderIndex)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        Quantity = quantity ?? Quantity;
        Unit = unit ?? Unit;
        Notes = notes ?? Notes;
        if (orderIndex.HasValue) OrderIndex = orderIndex.Value;
        Touch();
    }
}
