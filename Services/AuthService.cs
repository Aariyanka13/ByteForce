using Microsoft.AspNetCore.Identity;
using SmartRecruitmentMatchingPlatform.DTOs.Auth;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interfaces.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJobSeekerRepository _jobSeekerRepository;

    public AuthService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher,
    IJobSeekerRepository jobSeekerRepository)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _jobSeekerRepository = jobSeekerRepository;
    }

    public async Task<CurrentUserDto> RegisterJobSeekerAsync(
        RegisterJobSeekerDto request)
    {
        ValidatePassword(request.Password);

        var normalizedEmail = NormalizeEmail(request.Email);

        if (await _userRepository.EmailExistsAsync(normalizedEmail))
        {
            throw new ConflictException(
                "An account already exists with this email.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = UserRole.JobSeeker,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        var profile = new JobSeekerProfile
        {
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _jobSeekerRepository.AddAsync(profile);
        await _jobSeekerRepository.SaveChangesAsync();

        return MapCurrentUser(user);
    }

    public async Task<CurrentUserDto> RegisterEmployerAsync(
        RegisterEmployerDto request)
    {
        ValidatePassword(request.Password);

        var normalizedEmail = NormalizeEmail(request.Email);

        if (await _userRepository.EmailExistsAsync(normalizedEmail))
        {
            throw new ConflictException(
                "An account already exists with this email.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = UserRole.Employer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapCurrentUser(user);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository
            .GetByNormalizedEmailAsync(normalizedEmail);

        if (user is null)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException(
                "Your account has been disabled.");
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verificationResult ==
            PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var tokenResult =
            _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = MapCurrentUser(user)
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    private static void ValidatePassword(string password)
    {
        var hasUppercase = password.Any(char.IsUpper);
        var hasLowercase = password.Any(char.IsLower);
        var hasNumber = password.Any(char.IsDigit);

        if (password.Length < 8 ||
            !hasUppercase ||
            !hasLowercase ||
            !hasNumber)
        {
            throw new BadRequestException(
                "Password must contain at least 8 characters, " +
                "including uppercase, lowercase and a number.");
        }
    }

    private static CurrentUserDto MapCurrentUser(User user)
    {
        return new CurrentUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
