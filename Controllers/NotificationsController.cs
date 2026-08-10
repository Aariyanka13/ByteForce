using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Notifications;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationResponseDto>>> GetMine()
    {
        var userId = User.GetUserId();

        var result =
            await _notificationService.GetMyNotificationsAsync(userId);

        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadNotificationCountDto>> GetUnreadCount()
    {
        var userId = User.GetUserId();

        var result =
            await _notificationService.GetUnreadCountAsync(userId);

        return Ok(result);
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = User.GetUserId();

        await _notificationService.MarkAsReadAsync(
            userId,
            id);

        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.GetUserId();

        await _notificationService.MarkAllAsReadAsync(userId);

        return NoContent();
    }
}
