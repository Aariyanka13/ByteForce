using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interfaces.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}