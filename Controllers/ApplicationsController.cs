using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Applications;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(
        IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(
        [FromQuery] ApplicationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result =
            await _applicationService.GetMineAsync(
                User.GetUserId(),
                status,
                page,
                pageSize);

        return Ok(result);
    }

    [Authorize(Roles = "Employer")]
    [HttpGet("/api/vacancies/{vacancyId:int}/applicants")]
    public async Task<IActionResult> GetApplicants(
        int vacancyId)
    {
        var result =
            await _applicationService.GetApplicantsAsync(
                User.GetUserId(),
                vacancyId);

        return Ok(result);
    }

    [Authorize(Roles = "Employer")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateApplicationStatusDto request)
    {
        await _applicationService.UpdateStatusAsync(
            User.GetUserId(),
            id,
            request);

        return NoContent();
    }
}
