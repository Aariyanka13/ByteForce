using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interfaces.Repositories;

public interface IVacancyRepository
{
    Task<Vacancy?> GetByIdWithSkillsAsync(int vacancyId);

    Task<List<Vacancy>> GetByEmployerProfileIdAsync(
        int employerProfileId);

    Task AddAsync(Vacancy vacancy);

    Task SaveChangesAsync();
}
