using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.ContactRequests;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/contact-requests")]
[Authorize]
public class ContactRequestsController : ControllerBase
{
    private readonly IContactRequestService _contactRequestService;

    public ContactRequestsController(
        IContactRequestService contactRequestService)
    {
        _contactRequestService = contactRequestService;
    }

    [Authorize(Roles = "Employer")]
    [HttpPost]
    public async Task<ActionResult<ContactRequestResponseDto>> Create(
        [FromBody] CreateContactRequestDto request)
    {
        var userId = User.GetUserId();

        var result = await _contactRequestService.CreateAsync(
            userId,
            request);

        return Ok(result);
    }

    [Authorize(Roles = "Employer")]
    [HttpGet("employer")]
    public async Task<ActionResult<List<ContactRequestResponseDto>>>
        GetEmployerRequests()
    {
        var userId = User.GetUserId();

        var result =
            await _contactRequestService.GetEmployerRequestsAsync(
                userId);

        return Ok(result);
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpGet("jobseeker")]
    public async Task<ActionResult<List<ContactRequestResponseDto>>>
        GetJobSeekerRequests()
    {
        var userId = User.GetUserId();

        var result =
            await _contactRequestService.GetJobSeekerRequestsAsync(
                userId);

        return Ok(result);
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPut("{id:int}/respond")]
    public async Task<ActionResult<ContactRequestResponseDto>> Respond(
        int id,
        [FromBody] RespondContactRequestDto request)
    {
        var userId = User.GetUserId();

        var result = await _contactRequestService.RespondAsync(
            userId,
            id,
            request);

        return Ok(result);
    }

    [Authorize(Roles = "Employer")]
    [HttpGet("{id:int}/contact-details")]
    public async Task<ActionResult<ContactDetailsResponseDto>>
        GetContactDetails(int id)
    {
        var userId = User.GetUserId();

        var result =
            await _contactRequestService.GetContactDetailsAsync(
                userId,
                id);

        return Ok(result);
    }
}