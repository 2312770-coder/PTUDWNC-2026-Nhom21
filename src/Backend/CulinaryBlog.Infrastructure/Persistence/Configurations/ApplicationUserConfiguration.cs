using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

// SRS mục 7.7 - chỉ cấu hình các cột custom; các cột gốc của IdentityUser
// (Email, UserName, PasswordHash...) đã được EF Core Identity lo sẵn.
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.Bio).HasColumnType("text");
        builder.Property(u => u.IsActive).HasDefaultValue(true);
    }
}
