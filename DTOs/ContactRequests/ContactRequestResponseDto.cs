namespace SmartRecruitmentMatchingPlatform.DTOs.ContactRequests;

public class ContactRequestResponseDto
{
    public int Id { get; set; }

    public int JobApplicationId { get; set; }

    public int EmployerProfileId { get; set; }

    public int JobSeekerProfileId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public string VacancyTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string JobSeekerName { get; set; } = string.Empty;
}