using System.ComponentModel.DataAnnotations;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class UpdateJobSeekerProfileDto
{
    [StringLength(30)]
    public string? Phone { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Location { get; set; } = string.Empty;

    [Range(0, 50)]
    public decimal TotalExperienceYears { get; set; }

    [Required]
    public EducationLevel? EducationLevel { get; set; }

    [StringLength(1000)]
    public string? ProfileSummary { get; set; }
}