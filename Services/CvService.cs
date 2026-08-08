using Microsoft.Extensions.Options;
using SmartRecruitmentMatchingPlatform.DTOs.Cv;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Interface.Storage;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Options;

namespace SmartRecruitmentMatchingPlatform.Services;

public class CvService : ICvService
{
    private readonly IJobSeekerRepository _jobSeekerRepository;
    private readonly ICvDocumentRepository _cvRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly CvStorageOptions _options;

    public CvService(
        IJobSeekerRepository jobSeekerRepository,
        ICvDocumentRepository cvRepository,
        IFileStorageService fileStorage,
        IOptions<CvStorageOptions> options)
    {
        _jobSeekerRepository = jobSeekerRepository;
        _cvRepository = cvRepository;
        _fileStorage = fileStorage;
        _options = options.Value;
    }

    public async Task<CvDocumentResponseDto?> GetCurrentAsync(int userId)
    {
        var profile = await _jobSeekerRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var document = await _cvRepository
            .GetByJobSeekerProfileIdAsync(profile.Id);

        return document is null
            ? null
            : MapResponse(document);
    }

    public async Task<CvDocumentResponseDto> UploadAsync(
        int userId,
        IFormFile file)
    {
        FileValidationHelper.ValidateCvFile(file, _options);

        var profile = await _jobSeekerRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var existing = await _cvRepository
            .GetByJobSeekerProfileIdAsync(profile.Id);

        StoredFileResult? storedFile = null;

        try
        {
            storedFile = await _fileStorage.SaveAsync(file);

            if (existing is not null)
            {
                await _fileStorage.DeleteAsync(existing.RelativePath);
                _cvRepository.Remove(existing);
            }

            var document = new CvDocument
            {
                JobSeekerProfileId = profile.Id,
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFile.StoredFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                RelativePath = storedFile.RelativePath,
                UploadedAt = DateTime.UtcNow
            };

            await _cvRepository.AddAsync(document);
            await _cvRepository.SaveChangesAsync();

            return MapResponse(document);
        }
        catch
        {
            if (storedFile is not null)
            {
                await _fileStorage.DeleteAsync(
                    storedFile.RelativePath);
            }

            throw;
        }
    }

    public async Task<(
        Stream Stream,
        string ContentType,
        string FileName)> DownloadAsync(int userId)
    {
        var profile = await _jobSeekerRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var document = await _cvRepository
            .GetByJobSeekerProfileIdAsync(profile.Id)
            ?? throw new NotFoundException(
                "CV was not found.");

        var stream = await _fileStorage
            .OpenReadAsync(document.RelativePath);

        return (
            stream,
            document.ContentType,
            document.OriginalFileName);
    }

    public async Task DeleteAsync(int userId)
    {
        var profile = await _jobSeekerRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var document = await _cvRepository
            .GetByJobSeekerProfileIdAsync(profile.Id);

        if (document is null)
        {
            return;
        }

        await _fileStorage.DeleteAsync(document.RelativePath);

        _cvRepository.Remove(document);

        await _cvRepository.SaveChangesAsync();
    }

    private static CvDocumentResponseDto MapResponse(
        CvDocument document)
    {
        return new CvDocumentResponseDto
        {
            Id = document.Id,
            OriginalFileName = document.OriginalFileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            UploadedAt = document.UploadedAt
        };
    }
}