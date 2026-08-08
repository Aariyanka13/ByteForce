using SmartRecruitmentMatchingPlatform.DTOs.Skills;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;

    public SkillService(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<List<SkillResponseDto>> GetAllAsync(string? search)
    {
        var skills = await _skillRepository.GetAllAsync(search);

        return skills
            .Select(x => new SkillResponseDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();
    }
}