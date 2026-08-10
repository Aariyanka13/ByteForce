using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Common;
using SmartRecruitmentMatchingPlatform.DTOs.Jobs;
using SmartRecruitmentMatchingPlatform.DTOs.Applications;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = "JobSeeker")]
public class JobsController : ControllerBase
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IApplicationService _applicationService;

    public JobsController(
        IJobSearchService jobSearchService,
        IApplicationService applicationService)
    {
        _jobSearchService = jobSearchService;
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<JobListItemDto>>>
        Search(
            [FromQuery] JobSearchQueryDto query)
    {
        var result =
            await _jobSearchService.SearchAsync(
                User.GetUserId(),
                query);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobDetailsDto>>
        Get(int id)
    {
        var result =
            await _jobSearchService.GetDetailsAsync(
                User.GetUserId(),
                id);

        return Ok(result);
    }

    [HttpGet("{id:int}/match")]
    public async Task<ActionResult<MatchResultDto>>
        GetMatch(int id)
    {
        var result =
            await _jobSearchService.GetMatchAsync(
                User.GetUserId(),
                id);

        return Ok(result);
    }

    [HttpPost("{id:int}/applications")]
    public async Task<ActionResult<ApplicationListItemDto>>
        Apply(int id)
    {
        var result =
            await _applicationService.ApplyAsync(
                User.GetUserId(),
                id);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }
}
