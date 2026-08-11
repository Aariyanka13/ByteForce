using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    private DbSet<JobApplication> Applications
        => _context.Set<JobApplication>();

    public async Task<bool> ExistsAsync(
        int profileId,
        int vacancyId)
    {
        return await Applications.AnyAsync(a =>
            a.JobSeekerProfileId == profileId &&
            a.VacancyId == vacancyId);
    }

    public async Task<HashSet<int>> GetAppliedVacancyIdsAsync(
        int profileId,
        List<int> vacancyIds)
    {
        if (vacancyIds.Count == 0)
        {
            return new HashSet<int>();
        }

        var appliedIds = await Applications
            .AsNoTracking()
            .Where(a =>
                a.JobSeekerProfileId == profileId &&
                vacancyIds.Contains(a.VacancyId))
            .Select(a => a.VacancyId)
            .ToListAsync();

        return appliedIds.ToHashSet();
    }

    public async Task<int?> GetApplicationIdAsync(
        int profileId,
        int vacancyId)
    {
        return await Applications
            .AsNoTracking()
            .Where(a =>
                a.JobSeekerProfileId == profileId &&
                a.VacancyId == vacancyId)
            .Select(a => (int?)a.Id)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(
        JobApplication application)
    {
        await Applications.AddAsync(application);
    }

    public async Task<JobApplication?> GetByIdWithDetailsAsync(
        int applicationId)
    {
        return await Applications
            .AsNoTracking()
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.User)
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.CvDocument)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.EmployerProfile)
            .FirstOrDefaultAsync(a =>
                a.Id == applicationId);
    }

    public async Task<JobApplication?> GetOwnedByEmployerAsync(
        int applicationId,
        int employerProfileId)
    {
        return await Applications
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.User)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.EmployerProfile)
            .FirstOrDefaultAsync(a =>
                a.Id == applicationId &&
                a.Vacancy.EmployerProfileId ==
                    employerProfileId);
    }

    public async Task<List<JobApplication>> GetMineAsync(
        int profileId,
        ApplicationStatus? status,
        int page,
        int pageSize)
    {
        var query = Applications
            .AsNoTracking()
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.EmployerProfile)
            .Where(a =>
                a.JobSeekerProfileId == profileId);

        if (status.HasValue)
        {
            query = query.Where(a =>
                a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountMineAsync(
        int profileId,
        ApplicationStatus? status)
    {
        var query = Applications
            .AsNoTracking()
            .Where(a =>
                a.JobSeekerProfileId == profileId);

        if (status.HasValue)
        {
            query = query.Where(a =>
                a.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<List<JobApplication>> GetApplicantsAsync(
        int vacancyId)
    {
        return await Applications
            .AsNoTracking()
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.User)
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.CvDocument)
            .Where(a =>
                a.VacancyId == vacancyId)
            .OrderByDescending(a => a.MatchScore)
            .ThenBy(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetByVacancyAndCandidateAsync(
        int vacancyId,
        int jobSeekerProfileId)
    {
        return await Applications
            .AsNoTracking()
            .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.User)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.EmployerProfile)
            .FirstOrDefaultAsync(a =>
                a.VacancyId == vacancyId &&
                a.JobSeekerProfileId ==
                    jobSeekerProfileId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}