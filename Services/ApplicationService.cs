
using SmartRecruitmentMatchingPlatform.DTOs.Applications;
using SmartRecruitmentMatchingPlatform.DTOs.Common;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Services;

public class ApplicationService : IApplicationService
{
    private readonly IJobSeekerRepository _jobSeekerRepository;
    private readonly IJobSearchRepository _jobSearchRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMatchingService _matchingService;

    private readonly IEmployerProfileRepository
        _employerProfileRepository;

    private readonly IVacancyRepository _vacancyRepository;

    public ApplicationService(
        IJobSeekerRepository jobSeekerRepository,
        IJobSearchRepository jobSearchRepository,
        IApplicationRepository applicationRepository,
        IMatchingService matchingService,
        IEmployerProfileRepository employerProfileRepository,
        IVacancyRepository vacancyRepository)
    {
        _jobSeekerRepository = jobSeekerRepository;
        _jobSearchRepository = jobSearchRepository;
        _applicationRepository = applicationRepository;
        _matchingService = matchingService;
        _employerProfileRepository = employerProfileRepository;
        _vacancyRepository = vacancyRepository;
    }

    public async Task<ApplicationListItemDto> ApplyAsync(
        int jobSeekerUserId,
        int vacancyId)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(jobSeekerUserId);

        if (profile is null)
        {
            throw new NotFoundException(
                "Job seeker profile was not found.");
        }

        ValidateProfileForApplication(profile);

        var vacancy = await _jobSearchRepository
            .GetOpenWithDetailsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found or is closed.");
        }

        var alreadyApplied =
            await _applicationRepository.ExistsAsync(
                profile.Id,
                vacancyId);

        if (alreadyApplied)
        {
            throw new ConflictException(
                "You have already applied for this vacancy.");
        }

        var match = _matchingService.Calculate(
            profile,
            vacancy);

        var application = new JobApplication
        {
            JobSeekerProfileId = profile.Id,

            VacancyId = vacancy.Id,

            MatchScore =
                match.Breakdown.TotalScore,

            SkillScore =
                match.Breakdown.SkillScore,

            ExperienceScore =
                match.Breakdown.ExperienceScore,

            EducationScore =
                match.Breakdown.EducationScore,

            LocationScore =
                match.Breakdown.LocationScore,

            Status = ApplicationStatus.Applied,

            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepository.AddAsync(
            application);

        await _applicationRepository.SaveChangesAsync();

        return MapApplication(
            application,
            vacancy);
    }

    public async Task<PagedResultDto<ApplicationListItemDto>>
        GetMineAsync(
            int jobSeekerUserId,
            ApplicationStatus? status,
            int page,
            int pageSize)
    {
        ValidatePagination(
            ref page,
            ref pageSize);

        var profile = await _jobSeekerRepository
            .GetByUserIdAsync(jobSeekerUserId);

        if (profile is null)
        {
            throw new NotFoundException(
                "Job seeker profile was not found.");
        }

        var applications =
            await _applicationRepository.GetMineAsync(
                profile.Id,
                status,
                page,
                pageSize);

        var total =
            await _applicationRepository.CountMineAsync(
                profile.Id,
                status);

        return new PagedResultDto<ApplicationListItemDto>
        {
            Items = applications
                .Select(application =>
                    MapApplication(
                        application,
                        application.Vacancy))
                .ToList(),

            Page = page,

            PageSize = pageSize,

            TotalItems = total,

            TotalPages = total == 0
                ? 0
                : (int)Math.Ceiling(
                    total / (double)pageSize)
        };
    }

    public async Task<List<ApplicantListItemDto>>
        GetApplicantsAsync(
            int employerUserId,
            int vacancyId)
    {
        var employer =
            await _employerProfileRepository
                .GetByUserIdAsync(employerUserId);

        if (employer is null)
        {
            throw new NotFoundException(
                "Employer profile was not found.");
        }

        var vacancy =
            await _vacancyRepository
                .GetByIdWithSkillsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found.");
        }

        if (vacancy.EmployerProfileId != employer.Id)
        {
            throw new ForbiddenException(
                "You cannot view applicants for this vacancy.");
        }

        var applications =
            await _applicationRepository
                .GetApplicantsAsync(vacancyId);

        return applications
            .Select(MapApplicant)
            .ToList();
    }

    public async Task UpdateStatusAsync(
        int employerUserId,
        int applicationId,
        UpdateApplicationStatusDto request)
    {
        if (!Enum.IsDefined(
                typeof(ApplicationStatus),
                request.Status))
        {
            throw new BadRequestException(
                "Invalid application status.");
        }

        if (request.Status == ApplicationStatus.Applied)
        {
            throw new BadRequestException(
                "Applied is the initial application status " +
                "and cannot be selected by the employer.");
        }

        var employer =
            await _employerProfileRepository
                .GetByUserIdAsync(employerUserId);

        if (employer is null)
        {
            throw new NotFoundException(
                "Employer profile was not found.");
        }

        var application =
            await _applicationRepository
                .GetOwnedByEmployerAsync(
                    applicationId,
                    employer.Id);

        if (application is null)
        {
            throw new NotFoundException(
                "Application was not found.");
        }

        if (application.Status == request.Status)
        {
            return;
        }

        application.Status = request.Status;
        application.UpdatedAt = DateTime.UtcNow;

        await _applicationRepository.SaveChangesAsync();

        /*
         * Member 5 notification integration will be added here
         * after INotificationService becomes available.
         */
    }

    private static void ValidateProfileForApplication(
        JobSeekerProfile profile)
    {
        var missingItems = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.Location))
        {
            missingItems.Add("location");
        }

        if (!profile.EducationLevel.HasValue)
        {
            missingItems.Add("education");
        }

        if (!profile.JobSeekerSkills.Any())
        {
            missingItems.Add("skills");
        }

        if (profile.CvDocument is null)
        {
            missingItems.Add("CV");
        }

        if (missingItems.Count > 0)
        {
            throw new BadRequestException(
                "Complete your profile before applying. " +
                "Missing: " +
                string.Join(", ", missingItems));
        }
    }

    private static void ValidatePagination(
        ref int page,
        ref int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 50)
        {
            pageSize = 50;
        }
    }

    private static ApplicationListItemDto MapApplication(
        JobApplication application,
        Vacancy vacancy)
    {
        return new ApplicationListItemDto
        {
            ApplicationId = application.Id,

            VacancyId = vacancy.Id,

            JobTitle = vacancy.Title,

            CompanyName =
                vacancy.EmployerProfile?.CompanyName
                ?? string.Empty,

            MatchScore = application.MatchScore,

            Status = application.Status.ToString(),

            AppliedAt = application.AppliedAt,

            UpdatedAt = application.UpdatedAt
        };
    }

    private static ApplicantListItemDto MapApplicant(
        JobApplication application)
    {
        var profile = application.JobSeekerProfile;

        return new ApplicantListItemDto
        {
            ApplicationId = application.Id,

            JobSeekerProfileId = profile.Id,

            CandidateName =
                profile.User?.FullName
                ?? string.Empty,

            Location =
                profile.Location
                ?? string.Empty,

            ExperienceYears =
                profile.TotalExperienceYears,

            EducationLevel =
                profile.EducationLevel?.ToString()
                ?? "Not specified",

            MatchScore =
                application.MatchScore,

            Status =
                application.Status.ToString(),

            AppliedAt =
                application.AppliedAt
        };
    }
}