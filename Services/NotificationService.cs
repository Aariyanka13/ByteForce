using SmartRecruitmentMatchingPlatform.DTOs.Notifications;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync(
        int userId)
    {
        var notifications =
            await _notificationRepository.GetByUserIdAsync(userId);

        return notifications
            .Select(MapResponse)
            .ToList();
    }

    public async Task<UnreadNotificationCountDto> GetUnreadCountAsync(
        int userId)
    {
        var count =
            await _notificationRepository.GetUnreadCountAsync(userId);

        return new UnreadNotificationCountDto
        {
            Count = count
        };
    }

    public async Task MarkAsReadAsync(
        int userId,
        int notificationId)
    {
        var notification =
            await _notificationRepository.GetByIdAsync(notificationId)
            ?? throw new NotFoundException(
                "Notification was not found.");

        if (notification.UserId != userId)
        {
            throw new ForbiddenException(
                "You cannot modify this notification.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;

            await _notificationRepository.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications =
            await _notificationRepository.GetByUserIdAsync(userId);

        var unreadNotifications = notifications
            .Where(x => !x.IsRead)
            .ToList();

        if (unreadNotifications.Count == 0)
        {
            return;
        }

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _notificationRepository.SaveChangesAsync();
    }

    public async Task CreateAsync(
        int userId,
        NotificationType type,
        string title,
        string message,
        int? relatedEntityId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title.Trim(),
            Message = message.Trim(),
            RelatedEntityId = relatedEntityId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();
    }

    private static NotificationResponseDto MapResponse(
        Notification notification)
    {
        return new NotificationResponseDto
        {
            Id = notification.Id,
            Type = notification.Type.ToString(),
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            RelatedEntityId = notification.RelatedEntityId,
            CreatedAt = notification.CreatedAt
        };
    }
}