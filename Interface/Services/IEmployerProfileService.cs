using SmartRecruitmentMatchingPlatform.DTOs.Employers;

namespace SmartRecruitmentMatchingPlatform.Interfaces.Services;

public interface IEmployerProfileService
{
    Task<EmployerProfileResponseDto?> GetByUserIdAsync(int userId);

    Task<EmployerProfileResponseDto> CreateAsync(
        int userId,
        EmployerProfileRequestDto request);

    Task<EmployerProfileResponseDto> UpdateAsync(
        int userId,
        EmployerProfileRequestDto request);
}