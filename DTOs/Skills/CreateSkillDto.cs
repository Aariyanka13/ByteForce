using System.ComponentModel.DataAnnotations;

namespace SmartRecruitmentMatchingPlatform.DTOs.Skills;

public class CreateSkillDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
