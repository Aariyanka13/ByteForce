using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class CvDocumentConfiguration
    : IEntityTypeConfiguration<CvDocument>
{
    public void Configure(EntityTypeBuilder<CvDocument> builder)
    {
        builder.ToTable("CvDocuments");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.JobSeekerProfileId)
            .IsUnique();

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RelativePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.JobSeekerProfile)
            .WithOne(x => x.CvDocument)
            .HasForeignKey<CvDocument>(x => x.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}