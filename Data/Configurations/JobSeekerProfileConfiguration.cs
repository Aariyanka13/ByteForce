using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class JobSeekerProfileConfiguration
    : IEntityTypeConfiguration<JobSeekerProfile>
{
    public void Configure(EntityTypeBuilder<JobSeekerProfile> builder)
    {
        builder.ToTable("JobSeekerProfiles");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.Location)
            .HasMaxLength(150);

        builder.Property(x => x.TotalExperienceYears)
            .HasPrecision(4, 1);

        builder.Property(x => x.EducationLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.ProfileSummary)
            .HasMaxLength(1000);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<JobSeekerProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}