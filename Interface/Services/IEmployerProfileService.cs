using SmartRecruitmentMatchingPlatform.DTOs.Employers;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

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