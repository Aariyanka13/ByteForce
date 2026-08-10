using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface ICvDocumentRepository
{
    Task<CvDocument?> GetByJobSeekerProfileIdAsync(
        int jobSeekerProfileId);

    Task AddAsync(CvDocument document);

    void Remove(CvDocument document);

    Task SaveChangesAsync();
}
