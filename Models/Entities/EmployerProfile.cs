namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class EmployerProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Location { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
