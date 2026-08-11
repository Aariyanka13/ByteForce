namespace SmartRecruitmentMatchingPlatform.DTOs.Jobs;

public class JobDetailsDto
{
    public int VacancyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string CompanyLocation { get; set; } = string.Empty;

    public string JobLocation { get; set; } = string.Empty;

    public decimal RequiredExperienceYears { get; set; }

    public string RequiredEducationLevel { get; set; }
        = string.Empty;

    public List<string> RequiredSkills { get; set; } = new();

    public MatchResultDto Match { get; set; } = new();

    public bool HasApplied { get; set; }

    public int? ApplicationId { get; set; }

    public DateTime PostedAt { get; set; }
}