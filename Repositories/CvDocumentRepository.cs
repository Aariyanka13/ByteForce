using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class CvDocumentRepository : ICvDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public CvDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CvDocument?> GetByJobSeekerProfileIdAsync(
        int jobSeekerProfileId)
    {
        return await _context.CvDocuments
            .FirstOrDefaultAsync(x =>
                x.JobSeekerProfileId == jobSeekerProfileId);
    }

    public async Task AddAsync(CvDocument document)
    {
        await _context.CvDocuments.AddAsync(document);
    }

    public void Remove(CvDocument document)
    {
        _context.CvDocuments.Remove(document);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}