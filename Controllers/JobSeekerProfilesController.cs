using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.JobSeekers;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/jobseeker/profile")]
[Authorize(Roles = "JobSeeker")]
public class JobSeekerProfilesController : ControllerBase
{
    private readonly IJobSeekerProfileService _profileService;

    public JobSeekerProfilesController(
        IJobSeekerProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<JobSeekerProfileResponseDto>> GetCurrent()
    {
        var userId = User.GetUserId();

        var result = await _profileService.GetCurrentAsync(userId);

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<JobSeekerProfileResponseDto>> Update(
        [FromBody] UpdateJobSeekerProfileDto request)
    {
        var userId = User.GetUserId();

        var result = await _profileService.UpdateAsync(
            userId,
            request);

        return Ok(result);
    }
    [HttpGet("dashboard")]
    public async Task<ActionResult<JobSeekerDashboardDto>> GetDashboard()
    {
        var userId = User.GetUserId();

        var result = await _profileService.GetDashboardAsync(userId);

        return Ok(result);
    }

    [HttpPut("skills")]
    public async Task<ActionResult<JobSeekerProfileResponseDto>> UpdateSkills(
        [FromBody] UpdateJobSeekerSkillsDto request)
    {
        var userId = User.GetUserId();

        var result = await _profileService.UpdateSkillsAsync(
            userId,
            request);

        return Ok(result);
    }
}