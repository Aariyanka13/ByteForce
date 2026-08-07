using System.Security.Claims;

namespace SmartRecruitmentMatchingPlatform.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID is missing.");
        }

        return userId;
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(
            ClaimTypes.Role) ?? string.Empty;
    }
}
