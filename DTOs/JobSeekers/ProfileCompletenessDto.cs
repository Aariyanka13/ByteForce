namespace SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

public class ProfileCompletenessDto
{
    public int Percentage { get; set; }

    public List<string> MissingItems { get; set; } = new();
}