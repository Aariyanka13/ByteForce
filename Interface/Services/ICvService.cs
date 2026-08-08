using Microsoft.AspNetCore.Http;
using SmartRecruitmentMatchingPlatform.DTOs.Cv;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface ICvService
{
    Task<CvDocumentResponseDto?> GetCurrentAsync(int userId);

    Task<CvDocumentResponseDto> UploadAsync(
        int userId,
        IFormFile file);

    Task<(Stream Stream, string ContentType, string FileName)> DownloadAsync(
        int userId);

    Task DeleteAsync(int userId);
}