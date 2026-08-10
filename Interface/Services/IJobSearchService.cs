using SmartRecruitmentMatchingPlatform.DTOs.Common;
using SmartRecruitmentMatchingPlatform.DTOs.Jobs;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IJobSearchService
{
    Task<PagedResultDto<JobListItemDto>> SearchAsync(
        int jobSeekerUserId,
        JobSearchQueryDto query);

    Task<JobDetailsDto> GetDetailsAsync(
        int jobSeekerUserId,
        int vacancyId);

    Task<MatchResultDto> GetMatchAsync(
        int jobSeekerUserId,
        int vacancyId);
}