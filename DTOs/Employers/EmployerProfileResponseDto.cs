namespace SmartRecruitmentMatchingPlatform.DTOs.Employers;

public class EmployerProfileResponseDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Location { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}