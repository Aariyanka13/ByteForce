using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Skills;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/skills")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SkillResponseDto>>> GetAll(
        [FromQuery] string? search)
    {
        var result = await _skillService.GetAllAsync(search);

        return Ok(result);
    }
}