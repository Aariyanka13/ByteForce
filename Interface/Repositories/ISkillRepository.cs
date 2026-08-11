using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface ISkillRepository
{
    Task<List<Skill>> GetAllAsync(string? search);

    Task<bool> AllExistAsync(
        IReadOnlyCollection<int> skillIds);

    Task<Skill> GetOrCreateAsync(string name);
}