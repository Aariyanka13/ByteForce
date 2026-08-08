using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.DTOs.Vacancies;

public class VacancyResponseDto
{
    public int Id { get; set; }

    public int EmployerProfileId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal RequiredExperienceYears { get; set; }

    public EducationLevel RequiredEducationLevel { get; set; }

    public bool IsClosed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<int> SkillIds { get; set; } = new();
}
