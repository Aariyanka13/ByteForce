using System.ComponentModel.DataAnnotations;

namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class UpdateJobSeekerSkillsDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Select at least one skill.")]
    public List<int> SkillIds { get; set; } = new();
}