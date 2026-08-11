using SmartRecruitmentMatchingPlatform.DTOs.Jobs;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IMatchingService
{
    MatchResultDto Calculate(
        JobSeekerProfile? profile,
        Vacancy vacancy);
}