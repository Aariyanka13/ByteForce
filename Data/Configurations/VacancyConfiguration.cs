using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data.Configurations;

public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("Vacancies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(150);

        builder.Property(x => x.RequiredExperienceYears)
            .HasPrecision(4, 1);

        builder.Property(x => x.RequiredEducationLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(x => x.EmployerProfile)
            .WithMany()
            .HasForeignKey(x => x.EmployerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
