using System.ComponentModel.DataAnnotations;

namespace SmartRecruitmentMatchingPlatform.DTOs.Auth;

public class RegisterEmployerDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string CompanyName { get; set; } = string.Empty;
}
