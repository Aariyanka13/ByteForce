using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Employers;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/employer/profile")]
[Authorize(Roles = "Employer")]
public class EmployerProfilesController : ControllerBase
{
    private readonly IEmployerProfileService _employerProfileService;

    public EmployerProfilesController(
        IEmployerProfileService employerProfileService)
    {
        _employerProfileService = employerProfileService;
    }

    [HttpGet]
    public async Task<ActionResult<EmployerProfileResponseDto>> GetProfile()
    {
        var userId = User.GetUserId();

        var profile =
            await _employerProfileService.GetByUserIdAsync(userId);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<EmployerProfileResponseDto>> CreateProfile(
        [FromBody] EmployerProfileRequestDto request)
    {
        var userId = User.GetUserId();

        var profile =
            await _employerProfileService.CreateAsync(
                userId,
                request);

        return StatusCode(
            StatusCodes.Status201Created,
            profile);
    }

    [HttpPut]
    public async Task<ActionResult<EmployerProfileResponseDto>> UpdateProfile(
        [FromBody] EmployerProfileRequestDto request)
    {
        var userId = User.GetUserId();

        var profile =
            await _employerProfileService.UpdateAsync(
                userId,
                request);

        return Ok(profile);
    }
}
