using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

// SRS mục 7.5
public class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.ToTable("RecipeImages");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.OriginalUrl).IsRequired().HasMaxLength(500);
        builder.Property(i => i.MediumUrl).HasMaxLength(500);
        builder.Property(i => i.ThumbnailUrl).HasMaxLength(500);
        builder.Property(i => i.AltText).HasMaxLength(200);
        builder.Property(i => i.IsPrimary).HasDefaultValue(false);
        builder.Property(i => i.OrderIndex).HasDefaultValue(0);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.HasIndex(i => new { i.RecipeId, i.OrderIndex })
            .HasDatabaseName("IDX_RecipeImage_Recipe_Order");

        // SRS mục 7.5: mỗi Recipe chỉ có đúng 1 ảnh IsPrimary = true.
        // Partial unique index đảm bảo ràng buộc này ở tầng database.
        builder.HasIndex(i => i.RecipeId)
            .IsUnique()
            .HasFilter("\"IsPrimary\" = true AND \"IsDeleted\" = false")
            .HasDatabaseName("IDX_RecipeImage_OnePrimaryPerRecipe");
    }
}
