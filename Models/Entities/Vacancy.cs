using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class Vacancy
{
    public int Id { get; set; }

    public int EmployerProfileId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal RequiredExperienceYears { get; set; }

    public EducationLevel RequiredEducationLevel { get; set; }
        = EducationLevel.NoRequirement;

    public bool IsClosed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public EmployerProfile EmployerProfile { get; set; } = null!;

    public ICollection<VacancySkill> VacancySkills { get; set; }
    = new List<VacancySkill>();
}