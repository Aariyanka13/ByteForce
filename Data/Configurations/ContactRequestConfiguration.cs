using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class ContactRequestConfiguration
    : IEntityTypeConfiguration<ContactRequest>
{
    public void Configure(EntityTypeBuilder<ContactRequest> builder)
    {
        builder.ToTable("ContactRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.JobApplicationId,
            x.EmployerProfileId,
            x.JobSeekerProfileId
        })
        .IsUnique();

        builder.HasOne(x => x.JobApplication)
            .WithMany()
            .HasForeignKey(x => x.JobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EmployerProfile)
            .WithMany()
            .HasForeignKey(x => x.EmployerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JobSeekerProfile)
            .WithMany()
            .HasForeignKey(x => x.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}