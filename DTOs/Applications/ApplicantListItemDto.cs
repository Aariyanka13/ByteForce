namespace SmartRecruitmentMatchingPlatform.DTOs.Applications;

public class ApplicantListItemDto
{
    public int ApplicationId { get; set; }

    public int JobSeekerProfileId { get; set; }

    public string CandidateName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public decimal ExperienceYears { get; set; }

    public string EducationLevel { get; set; } = string.Empty;

    public decimal MatchScore { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime AppliedAt { get; set; }
}