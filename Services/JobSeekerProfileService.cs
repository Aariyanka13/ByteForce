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
    private readonly IApplicationRepository _applicationRepository;

    public JobSeekerProfileService(
        IJobSeekerRepository jobSeekerRepository,
        ISkillRepository skillRepository,
        IApplicationRepository applicationRepository)
    {
        _jobSeekerRepository = jobSeekerRepository;
        _skillRepository = skillRepository;
        _applicationRepository = applicationRepository;
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
    public async Task<JobSeekerDashboardDto> GetDashboardAsync(int userId)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var missingItems = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.Phone))
        {
            missingItems.Add("Phone");
        }

        if (string.IsNullOrWhiteSpace(profile.Location))
        {
            missingItems.Add("Location");
        }

        if (profile.EducationLevel is null)
        {
            missingItems.Add("Education Level");
        }

        if (string.IsNullOrWhiteSpace(profile.ProfileSummary))
        {
            missingItems.Add("Profile Summary");
        }

        if (!profile.JobSeekerSkills.Any())
        {
            missingItems.Add("Skills");
        }

        if (profile.CvDocument is null)
        {
            missingItems.Add("CV");
        }

        const int totalItems = 6;

        var completedItems =
            totalItems - missingItems.Count;

        var percentage =
            (int)Math.Round(
                completedItems * 100.0 / totalItems);

        var totalApps = await _applicationRepository.CountMineAsync(profile.Id, null);

        return new JobSeekerDashboardDto
        {
            Profile = MapProfile(profile),

            ProfileCompleteness = new ProfileCompletenessDto
            {
                Percentage = percentage,
                MissingItems = missingItems
            },

            SkillCount = profile.JobSeekerSkills.Count,

            HasCv = profile.CvDocument is not null,

            TotalApplications = totalApps
        };
    }

    private static JobSeekerProfileResponseDto MapProfile(
        JobSeekerProfile profile)
    {
        return new JobSeekerProfileResponseDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullName ?? string.Empty,
            Email = profile.User?.Email ?? string.Empty,
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