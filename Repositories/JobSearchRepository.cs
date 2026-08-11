
using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class JobSearchRepository : IJobSearchRepository
{
    private readonly ApplicationDbContext _context;

    public JobSearchRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Vacancy> Items, int Total)> SearchAsync(
        string? search,
        string? location,
        int? skillId,
        int page,
        int pageSize)
    {
        var query = _context.Vacancies
            .AsNoTracking()
            .Include(v => v.EmployerProfile)
            .Include(v => v.VacancySkills)
                .ThenInclude(vs => vs.Skill)
            .Where(v => !v.IsClosed);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(v =>
                v.Title.Contains(value) ||
                v.Description.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var value = location.Trim();

            query = query.Where(v =>
                v.Location != null &&
                v.Location.Contains(value));
        }

        if (skillId.HasValue)
        {
            query = query.Where(v =>
                v.VacancySkills.Any(vs =>
                    vs.SkillId == skillId.Value));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Vacancy?> GetOpenWithDetailsAsync(
        int vacancyId)
    {
        return await _context.Vacancies
            .AsNoTracking()
            .Include(v => v.EmployerProfile)
            .Include(v => v.VacancySkills)
                .ThenInclude(vs => vs.Skill)
            .FirstOrDefaultAsync(v =>
                v.Id == vacancyId &&
                !v.IsClosed);
    }
}