using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.NormalizedEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(user => user.NormalizedEmail)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .IsRequired();

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true);

        builder.Property(user => user.CreatedAt)
            .IsRequired();
    }
}