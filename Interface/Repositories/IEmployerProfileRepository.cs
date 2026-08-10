using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interfaces.Repositories;

public interface IEmployerProfileRepository
{
    Task<EmployerProfile?> GetByUserIdAsync(int userId);

    Task AddAsync(EmployerProfile employerProfile);

    Task SaveChangesAsync();
}
