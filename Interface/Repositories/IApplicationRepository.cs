using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface IApplicationRepository
{
    Task<bool> ExistsAsync(
        int profileId,
        int vacancyId);

    Task<HashSet<int>> GetAppliedVacancyIdsAsync(
        int profileId,
        List<int> vacancyIds);

    Task<int?> GetApplicationIdAsync(
        int profileId,
        int vacancyId);

    Task AddAsync(
        JobApplication application);

    Task<JobApplication?> GetByIdWithDetailsAsync(
        int applicationId);

    Task<JobApplication?> GetOwnedByEmployerAsync(
        int applicationId,
        int employerProfileId);

    Task<List<JobApplication>> GetMineAsync(
        int profileId,
        ApplicationStatus? status,
        int page,
        int pageSize);

    Task<int> CountMineAsync(
        int profileId,
        ApplicationStatus? status);

    Task<List<JobApplication>> GetApplicantsAsync(
        int vacancyId);

    Task<JobApplication?> GetByVacancyAndCandidateAsync(
        int vacancyId,
        int jobSeekerProfileId);

    Task SaveChangesAsync();
}