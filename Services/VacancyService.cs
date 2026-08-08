using SmartRecruitmentMatchingPlatform.DTOs.Vacancies;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Interfaces.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Services;

public class VacancyService : IVacancyService
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IEmployerProfileRepository _employerProfileRepository;
    private readonly ISkillRepository _skillRepository;

    public VacancyService(
        IVacancyRepository vacancyRepository,
        IEmployerProfileRepository employerProfileRepository,
        ISkillRepository skillRepository)
    {
        _vacancyRepository = vacancyRepository;
        _employerProfileRepository = employerProfileRepository;
        _skillRepository = skillRepository;
    }

    public async Task<List<VacancyResponseDto>> GetEmployerVacanciesAsync(
        int userId)
    {
        var employerProfile =
            await GetEmployerProfileAsync(userId);

        var vacancies =
            await _vacancyRepository.GetByEmployerProfileIdAsync(
                employerProfile.Id);

        return vacancies
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<VacancyResponseDto> GetByIdAsync(
        int userId,
        int vacancyId)
    {
        var employerProfile =
            await GetEmployerProfileAsync(userId);

        var vacancy =
            await _vacancyRepository.GetByIdWithSkillsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found.");
        }

        EnsureOwnership(vacancy, employerProfile.Id);

        return MapToResponse(vacancy);
    }

    public async Task<VacancyResponseDto> CreateAsync(
        int userId,
        CreateVacancyRequestDto request)
    {
        var employerProfile =
            await GetEmployerProfileAsync(userId);

        var skillIds = request.SkillIds
            .Distinct()
            .ToList();

        await ValidateSkillsAsync(skillIds);

        var vacancy = new Vacancy
        {
            EmployerProfileId = employerProfile.Id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Location = request.Location?.Trim(),
            RequiredExperienceYears =
                request.RequiredExperienceYears,
            RequiredEducationLevel =
                request.RequiredEducationLevel,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow,
            VacancySkills = skillIds
                .Select(skillId => new VacancySkill
                {
                    SkillId = skillId
                })
                .ToList()
        };

        await _vacancyRepository.AddAsync(vacancy);
        await _vacancyRepository.SaveChangesAsync();

        return MapToResponse(vacancy);
    }

    public async Task<VacancyResponseDto> UpdateAsync(
        int userId,
        int vacancyId,
        UpdateVacancyRequestDto request)
    {
        var employerProfile =
            await GetEmployerProfileAsync(userId);

        var vacancy =
            await _vacancyRepository.GetByIdWithSkillsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found.");
        }

        EnsureOwnership(vacancy, employerProfile.Id);

        if (vacancy.IsClosed)
        {
            throw new BadRequestException(
                "A closed vacancy cannot be updated.");
        }

        var skillIds = request.SkillIds
            .Distinct()
            .ToList();

        await ValidateSkillsAsync(skillIds);

        vacancy.Title = request.Title.Trim();
        vacancy.Description = request.Description.Trim();
        vacancy.Location = request.Location?.Trim();
        vacancy.RequiredExperienceYears =
            request.RequiredExperienceYears;
        vacancy.RequiredEducationLevel =
            request.RequiredEducationLevel;
        vacancy.UpdatedAt = DateTime.UtcNow;

        vacancy.VacancySkills.Clear();

        foreach (var skillId in skillIds)
        {
            vacancy.VacancySkills.Add(
                new VacancySkill
                {
                    VacancyId = vacancy.Id,
                    SkillId = skillId
                });
        }

        await _vacancyRepository.SaveChangesAsync();

        return MapToResponse(vacancy);
    }

    public async Task<VacancyResponseDto> CloseAsync(
        int userId,
        int vacancyId)
    {
        var employerProfile =
            await GetEmployerProfileAsync(userId);

        var vacancy =
            await _vacancyRepository.GetByIdWithSkillsAsync(vacancyId);

        if (vacancy is null)
        {
            throw new NotFoundException(
                "Vacancy was not found.");
        }

        EnsureOwnership(vacancy, employerProfile.Id);

        vacancy.IsClosed = true;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _vacancyRepository.SaveChangesAsync();

        return MapToResponse(vacancy);
    }

    private async Task<EmployerProfile> GetEmployerProfileAsync(
        int userId)
    {
        var employerProfile =
            await _employerProfileRepository.GetByUserIdAsync(userId);

        if (employerProfile is null)
        {
            throw new NotFoundException(
                "Employer profile was not found.");
        }

        return employerProfile;
    }

    private async Task ValidateSkillsAsync(
        IReadOnlyCollection<int> skillIds)
    {
        if (skillIds.Count == 0)
        {
            throw new BadRequestException(
                "At least one skill is required.");
        }

        var allSkillsExist =
            await _skillRepository.AllExistAsync(skillIds);

        if (!allSkillsExist)
        {
            throw new BadRequestException(
                "One or more selected skills do not exist.");
        }
    }

    private static void EnsureOwnership(
        Vacancy vacancy,
        int employerProfileId)
    {
        if (vacancy.EmployerProfileId != employerProfileId)
        {
            throw new ForbiddenException(
                "You cannot manage this vacancy.");
        }
    }

    private static VacancyResponseDto MapToResponse(
        Vacancy vacancy)
    {
        return new VacancyResponseDto
        {
            Id = vacancy.Id,
            EmployerProfileId = vacancy.EmployerProfileId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Location = vacancy.Location,
            RequiredExperienceYears =
                vacancy.RequiredExperienceYears,
            RequiredEducationLevel =
                vacancy.RequiredEducationLevel,
            IsClosed = vacancy.IsClosed,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            SkillIds = vacancy.VacancySkills
                .Select(x => x.SkillId)
                .ToList()
        };
    }
}
