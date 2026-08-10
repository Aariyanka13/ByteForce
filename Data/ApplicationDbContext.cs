using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<JobSeekerSkill> JobSeekerSkills => Set<JobSeekerSkill>();

    public DbSet<CvDocument> CvDocuments => Set<CvDocument>();

    public DbSet<EmployerProfile> EmployerProfiles => Set<EmployerProfile>();

    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<JobSeekerSkill> JobSeekerSkills => Set<JobSeekerSkill>();

    public DbSet<CvDocument> CvDocuments => Set<CvDocument>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    public DbSet<VacancySkill> VacancySkills => Set<VacancySkill>();

    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}