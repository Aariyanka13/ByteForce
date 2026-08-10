namespace SmartRecruitmentMatchingPlatform.DTOs.Jobs;

public class MatchBreakdownDto
{
    public decimal SkillScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    public decimal TotalScore { get; set; }
}