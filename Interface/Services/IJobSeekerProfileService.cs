using SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IJobSeekerProfileService
{
    Task<JobSeekerProfileResponseDto> GetCurrentAsync(int userId);

    Task<JobSeekerProfileResponseDto> UpdateAsync(
        int userId,
        UpdateJobSeekerProfileDto request);

    Task<JobSeekerProfileResponseDto> UpdateSkillsAsync(
        int userId,
        UpdateJobSeekerSkillsDto request);
}