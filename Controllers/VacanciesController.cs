using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Vacancies;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/vacancies")]
[Authorize(Roles = "Employer")]
public class VacanciesController : ControllerBase
{
    private readonly IVacancyService _vacancyService;

    public VacanciesController(
        IVacancyService vacancyService)
    {
        _vacancyService = vacancyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<VacancyResponseDto>>>
        GetMyVacancies()
    {
        var userId = User.GetUserId();

        var vacancies =
            await _vacancyService.GetEmployerVacanciesAsync(userId);

        return Ok(vacancies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VacancyResponseDto>>
        GetVacancy(int id)
    {
        var userId = User.GetUserId();

        var vacancy =
            await _vacancyService.GetByIdAsync(userId, id);

        return Ok(vacancy);
    }

    [HttpPost]
    public async Task<ActionResult<VacancyResponseDto>>
        CreateVacancy(
            [FromBody] CreateVacancyRequestDto request)
    {
        var userId = User.GetUserId();

        var vacancy =
            await _vacancyService.CreateAsync(
                userId,
                request);

        return StatusCode(
            StatusCodes.Status201Created,
            vacancy);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VacancyResponseDto>>
        UpdateVacancy(
            int id,
            [FromBody] UpdateVacancyRequestDto request)
    {
        var userId = User.GetUserId();

        var vacancy =
            await _vacancyService.UpdateAsync(
                userId,
                id,
                request);

        return Ok(vacancy);
    }

    [HttpPatch("{id:int}/close")]
    public async Task<ActionResult<VacancyResponseDto>>
        CloseVacancy(int id)
    {
        var userId = User.GetUserId();

        var vacancy =
            await _vacancyService.CloseAsync(
                userId,
                id);

        return Ok(vacancy);
    }
}
