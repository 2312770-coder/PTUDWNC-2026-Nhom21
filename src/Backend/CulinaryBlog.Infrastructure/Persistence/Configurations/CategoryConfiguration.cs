using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

// SRS mục 7.6
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Description).HasColumnType("text");
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.Property(c => c.OrderIndex).HasDefaultValue(0);
        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasIndex(c => c.Name).IsUnique().HasDatabaseName("IDX_Category_Name");
        builder.HasIndex(c => c.Slug).IsUnique().HasDatabaseName("IDX_Category_Slug");
    }
}
