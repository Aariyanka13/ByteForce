using SmartRecruitmentMatchingPlatform.Options;

namespace SmartRecruitmentMatchingPlatform.Helpers;

public static class FileValidationHelper
{
    public static void ValidateCvFile(
        IFormFile file,
        CvStorageOptions options)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException(
                "Please select a CV file.");
        }

        if (file.Length > options.MaximumFileSizeBytes)
        {
            throw new ArgumentException(
                "CV file size exceeds the allowed limit.");
        }

        var extension = Path
            .GetExtension(file.FileName)
            .ToLowerInvariant();

        if (!options.AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                "Only PDF and DOCX files are allowed.");
        }
    }
}