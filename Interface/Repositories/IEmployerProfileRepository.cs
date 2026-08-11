using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IEmployerProfileRepository
{
    Task<EmployerProfile?> GetByUserIdAsync(int userId);

    Task AddAsync(EmployerProfile employerProfile);

    Task SaveChangesAsync();
}
