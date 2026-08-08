using Microsoft.AspNetCore.Http;

namespace SmartRecruitmentMatchingPlatform.Interface.Storage;

public interface IFileStorageService
{
    Task<StoredFileResult> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default);
}