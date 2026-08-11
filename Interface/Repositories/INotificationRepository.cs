using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Interface.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(int userId);

    Task<Notification?> GetByIdAsync(int notificationId);

    Task<int> GetUnreadCountAsync(int userId);

    Task AddAsync(Notification notification);

    Task SaveChangesAsync();
}