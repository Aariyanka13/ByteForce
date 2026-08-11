using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IJobSeekerRepository
{
    Task<JobSeekerProfile?> GetByUserIdAsync(int userId);

    Task<JobSeekerProfile?> GetByIdAsync(int profileId);

    Task<JobSeekerProfile?> GetWithDetailsByUserIdAsync(int userId);
    Task AddAsync(JobSeekerProfile profile);

    Task UpdateSkillsAsync(
        JobSeekerProfile profile,
        IReadOnlyCollection<int> skillIds);

    Task SaveChangesAsync();
}