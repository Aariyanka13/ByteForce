using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitmentMatchingPlatform.DTOs.Auth;
using SmartRecruitmentMatchingPlatform.Helpers;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;

namespace SmartRecruitmentMatchingPlatform.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AuthController(
        IAuthService authService,
        IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    [HttpPost("register/jobseeker")]
    public async Task<ActionResult<CurrentUserDto>> RegisterJobSeeker(
        [FromBody] RegisterJobSeekerDto request)
    {
        var result =
            await _authService.RegisterJobSeekerAsync(request);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [AllowAnonymous]
    [HttpPost("register/employer")]
    public async Task<ActionResult<CurrentUserDto>> RegisterEmployer(
        [FromBody] RegisterEmployerDto request)
    {
        var result =
            await _authService.RegisterEmployerAsync(request);

        return StatusCode(
        StatusCodes.Status201Created,
        result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto request)
    {
        var result =
            await _authService.LoginAsync(request);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var userId = User.GetUserId();

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(new CurrentUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }
}