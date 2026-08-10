using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Admin;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
    {
        var result = await _adminService.GetDashboardAsync();

        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<AdminUserResponseDto>>> GetUsers(
        [FromQuery] string? search,
        [FromQuery] UserRole? role,
        [FromQuery] bool? isActive)
    {
        var result = await _adminService.GetUsersAsync(
            search,
            role,
            isActive);

        return Ok(result);
    }

    [HttpPut("users/{id:int}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        int id,
        [FromBody] UpdateUserStatusDto request)
    {
        var currentAdminUserId = User.GetUserId();

        await _adminService.UpdateUserStatusAsync(
            currentAdminUserId,
            id,
            request);

        return NoContent();
    }
}