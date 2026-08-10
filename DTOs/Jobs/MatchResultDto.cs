namespace SmartRecruitmentMatchingPlatform.DTOs.Jobs;

public class MatchResultDto
{
    public MatchBreakdownDto Breakdown { get; set; } = new();

    public List<MissingSkillDto> MissingSkills { get; set; } = new();

    public int MatchedSkillCount { get; set; }

    public int RequiredSkillCount { get; set; }
}