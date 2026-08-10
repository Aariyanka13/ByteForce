using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Data;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(int notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == notificationId);
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(x =>
                x.UserId == userId &&
                !x.IsRead);
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}