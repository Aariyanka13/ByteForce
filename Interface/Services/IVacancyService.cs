using SmartRecruitmentMatchingPlatform.DTOs.Vacancies;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IVacancyService
{
    Task<List<VacancyResponseDto>> GetEmployerVacanciesAsync(
        int userId);

    Task<VacancyResponseDto> GetByIdAsync(
        int userId,
        int vacancyId);

    Task<VacancyResponseDto> CreateAsync(
        int userId,
        CreateVacancyRequestDto request);

    Task<VacancyResponseDto> UpdateAsync(
        int userId,
        int vacancyId,
        UpdateVacancyRequestDto request);

    Task<VacancyResponseDto> CloseAsync(
        int userId,
        int vacancyId);
}
