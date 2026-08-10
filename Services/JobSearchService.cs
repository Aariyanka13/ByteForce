using SmartRecruitmentMatchingPlatform.DTOs.Common;
using SmartRecruitmentMatchingPlatform.DTOs.Jobs;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Services;

public class JobSearchService : IJobSearchService
{
    private readonly IJobSeekerRepository _jobSeekerRepository;
    private readonly IJobSearchRepository _jobSearchRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMatchingService _matchingService;

    public JobSearchService(
        IJobSeekerRepository jobSeekerRepository,
        IJobSearchRepository jobSearchRepository,
        IApplicationRepository applicationRepository,
        IMatchingService matchingService)
    {
        _jobSeekerRepository = jobSeekerRepository;
        _jobSearchRepository = jobSearchRepository;
        _applicationRepository = applicationRepository;
        _matchingService = matchingService;
    }

    public async Task<PagedResultDto<JobListItemDto>>
        SearchAsync(
            int jobSeekerUserId,
            JobSearchQueryDto query)
    {
        var profile = await _jobSeekerRepository
            .GetWithDetailsByUserIdAsync(jobSeekerUserId);

        if (profile is null)
        {
            throw new NotFoundException(
                "Job seeker profile was not found.");
        }

        var page = query.Page;
        var pageSize = query.PageSize;

        ValidatePagination(
            ref page,
            ref pageSize);

        var result = await _jobSearchRepository.SearchAsync(
            query.Search,
            query.Location,
            query.SkillId,
            page,
            pageSize);

        var items = new List<JobListItemDto>();

        foreach (var vacancy in result.Items)
        {
            var match = _matchingService.Calculate(
                profile,
                vacancy);

            var hasApplied =
                await _applicationRepository.ExistsAsync(
                    profile.Id,
                    vacancy.Id);

            items.Add(new JobListItemDto
            {
                VacancyId = vacancy.Id,

                Title = vacancy.Title,

                CompanyName =
                    vacancy.EmployerProfile?.CompanyName
                    ?? string.Empty,

                Location =
                    vacancy.Location
                    ?? string.Empty,

                MatchScore =
                    match.Breakdown.TotalScore,

                MissingSkillCount =
                    match.MissingSkills.Count,

                HasApplied = hasApplied
            });
        }

        return new PagedResultDto<JobListItemDto>
        {
            Items = items,

            Page = page,

            PageSize = pageSize,

            TotalItems = result.Total,

            TotalPages = result.Total == 0
                ? 0
                : (int)Math.Ceiling(
                    result.Total / (double)pageSize)
        };
    }

    public async Task<JobDetailsDto> GetDetailsAsync(
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

        var vacancy = await _jobSearchRepository
            .GetOpenWithDetailsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found or is closed.");
        }

        var match = _matchingService.Calculate(
            profile,
            vacancy);

        var applicationId =
            await _applicationRepository
                .GetApplicationIdAsync(
                    profile.Id,
                    vacancy.Id);

        return new JobDetailsDto
        {
            VacancyId = vacancy.Id,

            Title = vacancy.Title,

            Description = vacancy.Description,

            CompanyName =
                vacancy.EmployerProfile?.CompanyName
                ?? string.Empty,

            CompanyLocation =
                vacancy.EmployerProfile?.Location
                ?? string.Empty,

            JobLocation =
                vacancy.Location
                ?? string.Empty,

            RequiredExperienceYears =
                vacancy.RequiredExperienceYears,

            RequiredEducationLevel =
                vacancy.RequiredEducationLevel.ToString(),

            RequiredSkills =
                vacancy.VacancySkills
                    .Where(x => x.Skill != null)
                    .Select(x => x.Skill.Name)
                    .OrderBy(name => name)
                    .ToList(),

            Match = match,

            HasApplied =
                applicationId.HasValue,

            ApplicationId =
                applicationId
        };
    }

    public async Task<MatchResultDto> GetMatchAsync(
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

        var vacancy = await _jobSearchRepository
            .GetOpenWithDetailsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found or is closed.");
        }

        return _matchingService.Calculate(
            profile,
            vacancy);
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
}