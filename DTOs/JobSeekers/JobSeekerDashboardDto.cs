namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class JobSeekerDashboardDto
{
    public JobSeekerProfileResponseDto Profile { get; set; } = new();

    public ProfileCompletenessDto ProfileCompleteness { get; set; } = new();

    public int SkillCount { get; set; }

    public bool HasCv { get; set; }

    public int TotalApplications { get; set; }
}