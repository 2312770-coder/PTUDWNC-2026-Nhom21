using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

// SRS mục 7.4
public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Quantity).HasColumnType("decimal(10,3)"); // nullable
        builder.Property(i => i.Unit).HasMaxLength(50);
        builder.Property(i => i.Notes).HasMaxLength(500);
        builder.Property(i => i.OrderIndex).HasDefaultValue(0);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.HasIndex(i => new { i.RecipeId, i.OrderIndex })
            .HasDatabaseName("IDX_RecipeIngredient_Recipe_Order");
    }
}
