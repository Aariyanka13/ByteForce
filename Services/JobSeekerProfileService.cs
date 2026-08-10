using SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Services;

public class JobSeekerProfileService : IJobSeekerProfileService
{
    private readonly IJobSeekerRepository _jobSeekerRepository;
    private readonly ISkillRepository _skillRepository;

    public JobSeekerProfileService(
        IJobSeekerRepository jobSeekerRepository,
        ISkillRepository skillRepository)
    {
        _jobSeekerRepository = jobSeekerRepository;
        _skillRepository = skillRepository;
    }

    public async Task<JobSeekerProfileResponseDto> GetCurrentAsync(int userId)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        return MapProfile(profile);
    }

    public async Task<JobSeekerProfileResponseDto> UpdateAsync(
        int userId,
        UpdateJobSeekerProfileDto request)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        profile.Phone = string.IsNullOrWhiteSpace(request.Phone)
            ? null
            : request.Phone.Trim();

        profile.Location = request.Location.Trim();

        profile.TotalExperienceYears =
            request.TotalExperienceYears;

        profile.EducationLevel =
            request.EducationLevel;

        profile.ProfileSummary =
            string.IsNullOrWhiteSpace(request.ProfileSummary)
                ? null
                : request.ProfileSummary.Trim();

        profile.UpdatedAt = DateTime.UtcNow;

        await _jobSeekerRepository.SaveChangesAsync();

        return MapProfile(profile);
    }

    public async Task<JobSeekerProfileResponseDto> UpdateSkillsAsync(
        int userId,
        UpdateJobSeekerSkillsDto request)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        if (request.SkillIds.Count !=
            request.SkillIds.Distinct().Count())
        {
            throw new BadRequestException(
                "Duplicate skills are not allowed.");
        }

        if (!await _skillRepository
            .AllExistAsync(request.SkillIds))
        {
            throw new BadRequestException(
                "One or more selected skills do not exist.");
        }

        await _jobSeekerRepository.UpdateSkillsAsync(
            profile,
            request.SkillIds);

        profile.UpdatedAt = DateTime.UtcNow;

        await _jobSeekerRepository.SaveChangesAsync();

        var updatedProfile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        return MapProfile(updatedProfile);
    }

    private static JobSeekerProfileResponseDto MapProfile(
        JobSeekerProfile profile)
    {
        return new JobSeekerProfileResponseDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Phone = profile.Phone,
            Location = profile.Location,
            TotalExperienceYears =
                profile.TotalExperienceYears,
            EducationLevel =
                profile.EducationLevel,
            ProfileSummary =
                profile.ProfileSummary,
            CreatedAt =
                profile.CreatedAt,
            UpdatedAt =
                profile.UpdatedAt
        };
    }
}