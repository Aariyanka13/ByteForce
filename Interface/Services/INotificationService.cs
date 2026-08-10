using SmartRecruitmentMatchingPlatform.DTOs.Notifications;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface INotificationService
{
    Task<List<NotificationResponseDto>> GetMyNotificationsAsync(int userId);

    Task<UnreadNotificationCountDto> GetUnreadCountAsync(int userId);

    Task MarkAsReadAsync(int userId, int notificationId);

    Task MarkAllAsReadAsync(int userId);

    Task CreateAsync(
        int userId,
        NotificationType type,
        string title,
        string message,
        int? relatedEntityId = null);
}