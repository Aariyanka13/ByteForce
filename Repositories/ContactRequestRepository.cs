using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class ContactRequestRepository : IContactRequestRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContactRequest?> GetByIdAsync(int id)
    {
        return await _context.ContactRequests
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ContactRequest?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.ContactRequests
            .Include(x => x.JobApplication)
                .ThenInclude(x => x.JobSeekerProfile)
                    .ThenInclude(x => x.User)
            .Include(x => x.EmployerProfile)
                .ThenInclude(x => x.User)
            .Include(x => x.JobSeekerProfile)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsForApplicationAsync(
        int jobApplicationId,
        int employerProfileId,
        int jobSeekerProfileId)
    {
        return await _context.ContactRequests
            .AnyAsync(x =>
                x.JobApplicationId == jobApplicationId &&
                x.EmployerProfileId == employerProfileId &&
                x.JobSeekerProfileId == jobSeekerProfileId);
    }

    public async Task<List<ContactRequest>> GetByEmployerProfileIdAsync(
        int employerProfileId)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .Include(x => x.JobApplication)
                .ThenInclude(x => x.Vacancy)
            .Include(x => x.EmployerProfile)
            .Include(x => x.JobSeekerProfile)
                .ThenInclude(x => x.User)
            .Where(x => x.EmployerProfileId == employerProfileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ContactRequest>> GetByJobSeekerProfileIdAsync(
        int jobSeekerProfileId)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .Include(x => x.JobApplication)
                .ThenInclude(x => x.Vacancy)
            .Include(x => x.EmployerProfile)
            .Include(x => x.JobSeekerProfile)
                .ThenInclude(x => x.User)
            .Where(x => x.JobSeekerProfileId == jobSeekerProfileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ContactRequest contactRequest)
    {
        await _context.ContactRequests.AddAsync(contactRequest);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}