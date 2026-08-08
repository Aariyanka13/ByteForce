using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class VacancyRepository : IVacancyRepository
{
    private readonly ApplicationDbContext _context;

    public VacancyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vacancy?> GetByIdWithSkillsAsync(int vacancyId)
    {
        return await _context.Vacancies
            .Include(v => v.VacancySkills)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
    }

    public async Task<List<Vacancy>> GetByEmployerProfileIdAsync(
        int employerProfileId)
    {
        return await _context.Vacancies
            .Include(v => v.VacancySkills)
            .Where(v => v.EmployerProfileId == employerProfileId)
            .ToListAsync();
    }

    public async Task AddAsync(Vacancy vacancy)
    {
        await _context.Vacancies.AddAsync(vacancy);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
