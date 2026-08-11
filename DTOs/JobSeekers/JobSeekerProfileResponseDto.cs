using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class JobSeekerProfileResponseDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public decimal TotalExperienceYears { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    public string? ProfileSummary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}