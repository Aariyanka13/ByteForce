using SmartRecruitmentMatchingPlatform.DTOs.Employers;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Interfaces.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Services;

public class EmployerProfileService : IEmployerProfileService
{
    private readonly IEmployerProfileRepository _employerProfileRepository;

    public EmployerProfileService(
        IEmployerProfileRepository employerProfileRepository)
    {
        _employerProfileRepository = employerProfileRepository;
    }

    public async Task<EmployerProfileResponseDto?> GetByUserIdAsync(int userId)
    {
        var profile = await _employerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            return null;
        }

        return MapToResponse(profile);
    }

    public async Task<EmployerProfileResponseDto> CreateAsync(
        int userId,
        EmployerProfileRequestDto request)
    {
        var existingProfile = await _employerProfileRepository
            .GetByUserIdAsync(userId);

        if (existingProfile is not null)
        {
            throw new ConflictException(
                "Employer profile already exists.");
        }

        var profile = new EmployerProfile
        {
            UserId = userId,
            CompanyName = request.CompanyName.Trim(),
            Industry = request.Industry?.Trim(),
            Location = request.Location?.Trim(),
            Website = request.Website?.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _employerProfileRepository.AddAsync(profile);
        await _employerProfileRepository.SaveChangesAsync();

        return MapToResponse(profile);
    }

    public async Task<EmployerProfileResponseDto> UpdateAsync(
        int userId,
        EmployerProfileRequestDto request)
    {
        var profile = await _employerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            throw new NotFoundException(
                "Employer profile was not found.");
        }

        profile.CompanyName = request.CompanyName.Trim();
        profile.Industry = request.Industry?.Trim();
        profile.Location = request.Location?.Trim();
        profile.Website = request.Website?.Trim();
        profile.Description = request.Description?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        await _employerProfileRepository.SaveChangesAsync();

        return MapToResponse(profile);
    }

    private static EmployerProfileResponseDto MapToResponse(
        EmployerProfile profile)
    {
        return new EmployerProfileResponseDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            CompanyName = profile.CompanyName,
            Industry = profile.Industry,
            Location = profile.Location,
            Website = profile.Website,
            Description = profile.Description,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}