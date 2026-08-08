namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class JobSeekerSummaryDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal TotalExperienceYears { get; set; }

    public string? EducationLevel { get; set; }
}