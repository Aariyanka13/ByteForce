using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<JobSeekerSkill> JobSeekerSkills => Set<JobSeekerSkill>();

    public DbSet<CvDocument> CvDocuments => Set<CvDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }


}