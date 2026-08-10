using System.ComponentModel.DataAnnotations;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    [Required]
    public ApplicationStatus Status { get; set; }
}