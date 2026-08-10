namespace SmartRecruitmentMatchingPlatform.DTOs.Applications;

public class ApplicationListItemDto
{
    public int ApplicationId { get; set; }

    public int VacancyId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public decimal MatchScore { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}