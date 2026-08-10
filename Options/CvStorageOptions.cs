namespace SmartRecruitmentMatchingPlatform.Options;

public class CvStorageOptions
{
    public const string SectionName = "CvStorage";

    public string RootPath { get; set; } = "ProtectedFiles/CVs";

    public long MaximumFileSizeBytes { get; set; }
        = 5 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; }
        = [".pdf", ".docx"];
}