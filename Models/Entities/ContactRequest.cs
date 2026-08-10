using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class ContactRequest
{
    public int Id { get; set; }

    public int JobApplicationId { get; set; }

    public int EmployerProfileId { get; set; }

    public int JobSeekerProfileId { get; set; }

    public ContactRequestStatus Status { get; set; }
        = ContactRequestStatus.Pending;

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? RespondedAt { get; set; }

    public JobApplication JobApplication { get; set; }
        = null!;

    public EmployerProfile EmployerProfile { get; set; }
        = null!;

    public JobSeekerProfile JobSeekerProfile { get; set; }
        = null!;
}