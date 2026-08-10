using SmartRecruitmentMatchingPlatform.DTOs.Skills;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface ISkillService
{
    Task<List<SkillResponseDto>> GetAllAsync(string? search);
}
