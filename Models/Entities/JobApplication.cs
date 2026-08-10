using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class JobApplication
{
    public int Id { get; set; }

    public int JobSeekerProfileId { get; set; }

    public int VacancyId { get; set; }

    public decimal MatchScore { get; set; }

    public decimal SkillScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    public ApplicationStatus Status { get; set; }
        = ApplicationStatus.Applied;

    public DateTime AppliedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public JobSeekerProfile JobSeekerProfile { get; set; }
        = null!;

    public Vacancy Vacancy { get; set; }
        = null!;
}