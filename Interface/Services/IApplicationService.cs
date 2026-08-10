using SmartRecruitmentMatchingPlatform.DTOs.Applications;
using SmartRecruitmentMatchingPlatform.DTOs.Common;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IApplicationService
{
    Task<ApplicationListItemDto> ApplyAsync(
        int jobSeekerUserId,
        int vacancyId);

    Task<PagedResultDto<ApplicationListItemDto>> GetMineAsync(
        int jobSeekerUserId,
        ApplicationStatus? status,
        int page,
        int pageSize);

    Task<List<ApplicantListItemDto>> GetApplicantsAsync(
        int employerUserId,
        int vacancyId);

    Task UpdateStatusAsync(
        int employerUserId,
        int applicationId,
        UpdateApplicationStatusDto request);
}