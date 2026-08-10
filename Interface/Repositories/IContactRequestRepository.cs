using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IContactRequestRepository
{
    Task<ContactRequest?> GetByIdAsync(int id);

    Task<ContactRequest?> GetByIdWithDetailsAsync(int id);

    Task<bool> ExistsForApplicationAsync(
        int jobApplicationId,
        int employerProfileId,
        int jobSeekerProfileId);

    Task<List<ContactRequest>> GetByEmployerProfileIdAsync(
        int employerProfileId);

    Task<List<ContactRequest>> GetByJobSeekerProfileIdAsync(
        int jobSeekerProfileId);

    Task AddAsync(ContactRequest contactRequest);

    Task SaveChangesAsync();
}