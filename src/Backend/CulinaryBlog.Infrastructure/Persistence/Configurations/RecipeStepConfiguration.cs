using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

// SRS mục 7.3
public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("RecipeSteps");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).IsRequired().HasColumnType("text");
        builder.Property(s => s.ImageUrl).HasMaxLength(500);
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_RecipeSteps_StepNumber", "\"StepNumber\" > 0");
            t.HasCheckConstraint("CK_RecipeSteps_TimerMinutes", "\"TimerMinutes\" IS NULL OR \"TimerMinutes\" >= 0");
        });

        // SRS mục 7.3: StepNumber unique trong phạm vi mỗi Recipe.
        builder.HasIndex(s => new { s.RecipeId, s.StepNumber })
            .IsUnique()
            .HasDatabaseName("IDX_RecipeStep_Recipe_StepNumber");
    }
}
