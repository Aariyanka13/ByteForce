using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class EmployerProfileRepository : IEmployerProfileRepository
{
    private readonly ApplicationDbContext _context;

    public EmployerProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployerProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.EmployerProfiles
            .FirstOrDefaultAsync(profile => profile.UserId == userId);
    }

    public async Task AddAsync(EmployerProfile employerProfile)
    {
        await _context.EmployerProfiles.AddAsync(employerProfile);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}