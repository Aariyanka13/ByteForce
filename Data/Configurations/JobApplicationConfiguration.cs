using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class JobApplicationConfiguration
    : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(
        EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.JobSeekerProfileId,
            x.VacancyId
        })
        .IsUnique();

        builder.Property(x => x.MatchScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.SkillScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.ExperienceScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.EducationScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.LocationScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.HasOne(x => x.JobSeekerProfile)
            .WithMany()
            .HasForeignKey(x => x.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.VacancyId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.AppliedAt);
    }
}