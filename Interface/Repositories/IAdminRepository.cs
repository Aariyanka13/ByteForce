using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IAdminRepository
{
    Task<List<User>> GetUsersAsync(
        string? search,
        UserRole? role,
        bool? isActive);

    Task<User?> GetUserByIdAsync(int userId);

    Task<int> GetTotalUsersCountAsync();

    Task<int> GetUsersByRoleCountAsync(UserRole role);

    Task<int> GetActiveUsersCountAsync();

    Task<int> GetDisabledUsersCountAsync();

    Task SaveChangesAsync();
}
