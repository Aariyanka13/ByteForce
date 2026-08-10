using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Cv;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/cv")]
[Authorize(Roles = "JobSeeker")]
public class CvController : ControllerBase
{
    private readonly ICvService _cvService;

    public CvController(ICvService cvService)
    {
        _cvService = cvService;
    }

    [HttpGet]
    public async Task<ActionResult<CvDocumentResponseDto?>> GetCurrent()
    {
        var userId = User.GetUserId();

        var result = await _cvService.GetCurrentAsync(userId);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CvDocumentResponseDto>> Upload(
        IFormFile file)
    {
        var userId = User.GetUserId();

        var result = await _cvService.UploadAsync(
            userId,
            file);

        return Ok(result);
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download()
    {
        var userId = User.GetUserId();

        var result = await _cvService.DownloadAsync(userId);

        return File(
            result.Stream,
            result.ContentType,
            result.FileName);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var userId = User.GetUserId();

        await _cvService.DeleteAsync(userId);

        return NoContent();
    }
}