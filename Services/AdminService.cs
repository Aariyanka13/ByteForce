using SmartRecruitmentMatchingPlatform.DTOs.Admin;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<List<AdminUserResponseDto>> GetUsersAsync(
        string? search,
        UserRole? role,
        bool? isActive)
    {
        var users = await _adminRepository.GetUsersAsync(
            search,
            role,
            isActive);

        return users
            .Select(MapUser)
            .ToList();
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        return new AdminDashboardDto
        {
            TotalUsers =
                await _adminRepository.GetTotalUsersCountAsync(),

            TotalJobSeekers =
                await _adminRepository.GetUsersByRoleCountAsync(
                    UserRole.JobSeeker),

            TotalEmployers =
                await _adminRepository.GetUsersByRoleCountAsync(
                    UserRole.Employer),

            ActiveUsers =
                await _adminRepository.GetActiveUsersCountAsync(),

            DisabledUsers =
                await _adminRepository.GetDisabledUsersCountAsync()
        };
    }

    public async Task UpdateUserStatusAsync(
        int currentAdminUserId,
        int userId,
        UpdateUserStatusDto request)
    {
        if (currentAdminUserId == userId)
        {
            throw new BadRequestException(
                "You cannot change the status of your own account.");
        }

        var user = await _adminRepository.GetUserByIdAsync(userId)
            ?? throw new NotFoundException(
                "User was not found.");

        user.IsActive = request.IsActive;

        await _adminRepository.SaveChangesAsync();
    }

    private static AdminUserResponseDto MapUser(User user)
    {
        return new AdminUserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}