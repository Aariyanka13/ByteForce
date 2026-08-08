using Microsoft.Extensions.Options;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Storage;
using SmartRecruitmentMatchingPlatform.Options;

namespace SmartRecruitmentMatchingPlatform.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IOptions<CvStorageOptions> options)
    {
        _rootPath = Path.GetFullPath(
            Path.Combine(
                environment.ContentRootPath,
                options.Value.RootPath));

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredFileResult> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var extension = Path
            .GetExtension(file.FileName)
            .ToLowerInvariant();

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var fullPath = Path.Combine(
            _rootPath,
            storedFileName);

        await using var stream =
            new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

        await file.CopyToAsync(
            stream,
            cancellationToken);

        return new StoredFileResult
        {
            StoredFileName = storedFileName,
            RelativePath = storedFileName
        };
    }

    public Task<Stream> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(relativePath);

        if (!File.Exists(fullPath))
        {
            throw new NotFoundException(
                "CV file was not found.");
        }

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetSafeFullPath(string relativePath)
    {
        var fullPath = Path.GetFullPath(
            Path.Combine(_rootPath, relativePath));

        if (!fullPath.StartsWith(
                _rootPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException(
                "Invalid file path.");
        }

        return fullPath;
    }
}