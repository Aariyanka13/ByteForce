using SmartRecruitmentMatchingPlatform.DTOs.Admin;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IAdminService
{
    Task<List<AdminUserResponseDto>> GetUsersAsync(
        string? search,
        UserRole? role,
        bool? isActive);

    Task<AdminDashboardDto> GetDashboardAsync();

    Task UpdateUserStatusAsync(
        int currentAdminUserId,
        int userId,
        UpdateUserStatusDto request);
}