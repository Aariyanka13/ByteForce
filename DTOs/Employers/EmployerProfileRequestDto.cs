using System.ComponentModel.DataAnnotations;

namespace SmartRecruitmentMatchingPlatform.DTOs.Employers;

public class EmployerProfileRequestDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Industry { get; set; }

    [StringLength(150)]
    public string? Location { get; set; }

    [StringLength(250)]
    [Url]
    public string? Website { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}
