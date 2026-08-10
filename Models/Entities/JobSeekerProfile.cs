using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class JobSeekerProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public decimal TotalExperienceYears { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    public string? ProfileSummary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
        = new List<JobSeekerSkill>();

    public CvDocument? CvDocument { get; set; }
}