namespace SmartRecruitmentMatchingPlatform.DTOs.Jobs;

public class JobSearchQueryDto
{
    public string? Search { get; set; }

    public string? Location { get; set; }

    public int? SkillId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}