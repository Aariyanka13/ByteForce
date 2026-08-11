using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _context;

    public SkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skill>> GetAllAsync(string? search)
    {
        var query = _context.Skills
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(skill =>
                skill.Name.Contains(value));
        }

        return await query
            .OrderBy(skill => skill.Name)
            .ToListAsync();
    }

    public async Task<bool> AllExistAsync(
        IReadOnlyCollection<int> skillIds)
    {
        var distinctIds = skillIds
            .Distinct()
            .ToList();

        var existingCount = await _context.Skills
            .CountAsync(skill =>
                distinctIds.Contains(skill.Id));

        return existingCount == distinctIds.Count;
    }

    public async Task<Skill> GetOrCreateAsync(string name)
    {
        var trimmedName = name.Trim();
        var normalized = trimmedName.ToUpperInvariant();

        var existing = await _context.Skills
            .FirstOrDefaultAsync(s => s.NormalizedName == normalized);

        if (existing != null)
        {
            return existing;
        }

        var newSkill = new Skill
        {
            Name = trimmedName,
            NormalizedName = normalized
        };

        _context.Skills.Add(newSkill);
        await _context.SaveChangesAsync();

        return newSkill;
    }
}