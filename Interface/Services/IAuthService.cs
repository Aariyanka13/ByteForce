using SmartRecruitmentMatchingPlatform.DTOs.Auth;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IAuthService
{
    Task<CurrentUserDto> RegisterJobSeekerAsync(
        RegisterJobSeekerDto request);

    Task<CurrentUserDto> RegisterEmployerAsync(
        RegisterEmployerDto request);

    Task<AuthResponseDto> LoginAsync(
        LoginDto request);
}