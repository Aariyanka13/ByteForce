using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByNormalizedEmailAsync(string normalizedEmail);

    Task<bool> EmailExistsAsync(string normalizedEmail);

    Task AddAsync(User user);

    Task SaveChangesAsync();
}