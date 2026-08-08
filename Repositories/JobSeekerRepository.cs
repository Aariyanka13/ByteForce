using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class JobSeekerRepository : IJobSeekerRepository
{
    private readonly ApplicationDbContext _context;

    public JobSeekerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobSeekerProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.JobSeekerProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<JobSeekerProfile?> GetByIdAsync(int profileId)
    {
        return await _context.JobSeekerProfiles
            .FirstOrDefaultAsync(x => x.Id == profileId);
    }

    public async Task<JobSeekerProfile?> GetWithDetailsByUserIdAsync(int userId)
    {
        return await _context.JobSeekerProfiles
            .Include(x => x.User)
            .Include(x => x.JobSeekerSkills)
                .ThenInclude(x => x.Skill)
            .Include(x => x.CvDocument)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task AddAsync(JobSeekerProfile profile)
    {
        await _context.JobSeekerProfiles.AddAsync(profile);
    }

    public async Task UpdateSkillsAsync(
        JobSeekerProfile profile,
        IReadOnlyCollection<int> skillIds)
    {
        _context.JobSeekerSkills.RemoveRange(
            profile.JobSeekerSkills);

        var items = skillIds
            .Select(skillId => new JobSeekerSkill
            {
                JobSeekerProfileId = profile.Id,
                SkillId = skillId
            })
            .ToList();

        await _context.JobSeekerSkills.AddRangeAsync(items);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}