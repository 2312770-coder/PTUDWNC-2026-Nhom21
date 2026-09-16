using CulinaryBlog.Domain.Common;
using SlugVO = CulinaryBlog.Domain.ValueObjects.Slug;

namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.6 - danh mục công thức. Name và Slug đều UNIQUE.
public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public int OrderIndex { get; private set; }

    public ICollection<Recipe> Recipes { get; private set; } = new List<Recipe>();

    protected Category() { }

    public static Category Create(string name, string? description = null,
        string? imageUrl = null, int orderIndex = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        return new Category
        {
            Name = name.Trim(),
            Slug = SlugVO.Create(name),
            Description = description?.Trim(),
            ImageUrl = imageUrl,
            OrderIndex = orderIndex,
        };
    }

    public void Update(string? name, string? description, string? imageUrl, int? orderIndex)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name.Trim();
            Slug = SlugVO.Create(name); // đổi tên thì sinh lại slug
        }
        Description = description ?? Description;
        ImageUrl = imageUrl ?? ImageUrl;
        if (orderIndex.HasValue) OrderIndex = orderIndex.Value;
        Touch();
    }
}
