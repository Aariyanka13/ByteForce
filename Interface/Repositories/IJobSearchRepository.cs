
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IJobSearchRepository
{
    Task<(List<Vacancy> Items, int Total)> SearchAsync(
        string? search,
        string? location,
        int? skillId,
        int page,
        int pageSize);

    Task<Vacancy?> GetOpenWithDetailsAsync(int vacancyId);
}