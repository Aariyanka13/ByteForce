using System.ComponentModel.DataAnnotations;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.DTOs.Vacancies;

public class CreateVacancyRequestDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Location { get; set; }

    [Range(0, 50)]
    public decimal RequiredExperienceYears { get; set; }

    public EducationLevel RequiredEducationLevel { get; set; }
        = EducationLevel.NoRequirement;

    [Required]
    [MinLength(1)]
    public List<int> SkillIds { get; set; } = new();
}
