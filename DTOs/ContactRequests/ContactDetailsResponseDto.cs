namespace SmartRecruitmentMatchingPlatform.DTOs.ContactRequests;

public class ContactDetailsResponseDto
{
    public int ContactRequestId { get; set; }

    public string JobSeekerName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
}