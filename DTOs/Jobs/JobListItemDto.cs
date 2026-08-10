namespace SmartRecruitmentMatchingPlatform.DTOs.Jobs;

public class JobListItemDto
{
    public int VacancyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public decimal MatchScore { get; set; }

    public int MissingSkillCount { get; set; }

    public bool HasApplied { get; set; }
}