using SmartRecruitmentMatchingPlatform.DTOs.ContactRequests;
using SmartRecruitmentMatchingPlatform.Exceptions;
using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Services;

public class ContactRequestService : IContactRequestService
{
    private readonly IContactRequestRepository _contactRequestRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IEmployerProfileRepository _employerProfileRepository;
    private readonly IJobSeekerRepository _jobSeekerRepository;
    private readonly INotificationService _notificationService;

    public ContactRequestService(
        IContactRequestRepository contactRequestRepository,
        IApplicationRepository applicationRepository,
        IEmployerProfileRepository employerProfileRepository,
        IJobSeekerRepository jobSeekerRepository,
        INotificationService notificationService)
    {
        _contactRequestRepository = contactRequestRepository;
        _applicationRepository = applicationRepository;
        _employerProfileRepository = employerProfileRepository;
        _jobSeekerRepository = jobSeekerRepository;
        _notificationService = notificationService;
    }

    public async Task<ContactRequestResponseDto> CreateAsync(
        int employerUserId,
        CreateContactRequestDto request)
    {
        var employerProfile =
            await _employerProfileRepository.GetByUserIdAsync(employerUserId)
            ?? throw new NotFoundException(
                "Employer profile was not found.");

        var application =
            await _applicationRepository.GetOwnedByEmployerAsync(
                request.JobApplicationId,
                employerProfile.Id)
            ?? throw new NotFoundException(
                "Application was not found.");

        if (application.Status != ApplicationStatus.Selected)
        {
            throw new BadRequestException(
                "Contact requests can only be sent for selected applications.");
        }

        var exists =
            await _contactRequestRepository.ExistsForApplicationAsync(
                application.Id,
                employerProfile.Id,
                application.JobSeekerProfileId);

        if (exists)
        {
            throw new ConflictException(
                "A contact request already exists for this application.");
        }

        var contactRequest = new ContactRequest
        {
            JobApplicationId = application.Id,
            EmployerProfileId = employerProfile.Id,
            JobSeekerProfileId = application.JobSeekerProfileId,
            Status = ContactRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _contactRequestRepository.AddAsync(contactRequest);
        await _contactRequestRepository.SaveChangesAsync();

        await _notificationService.CreateAsync(
            application.JobSeekerProfile.UserId,
            NotificationType.ContactRequestReceived,
            "New contact request",
            "An employer has sent you a contact request.",
            contactRequest.Id);

        return MapResponse(contactRequest);
    }

    public async Task<List<ContactRequestResponseDto>> GetEmployerRequestsAsync(
        int employerUserId)
    {
        var employerProfile =
            await _employerProfileRepository.GetByUserIdAsync(employerUserId)
            ?? throw new NotFoundException(
                "Employer profile was not found.");

        var requests =
            await _contactRequestRepository.GetByEmployerProfileIdAsync(
                employerProfile.Id);

        return requests
            .Select(MapResponse)
            .ToList();
    }

    public async Task<List<ContactRequestResponseDto>> GetJobSeekerRequestsAsync(
        int jobSeekerUserId)
    {
        var jobSeekerProfile =
            await _jobSeekerRepository.GetByUserIdAsync(jobSeekerUserId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var requests =
            await _contactRequestRepository.GetByJobSeekerProfileIdAsync(
                jobSeekerProfile.Id);

        return requests
            .Select(MapResponse)
            .ToList();
    }

    public async Task<ContactRequestResponseDto> RespondAsync(
        int jobSeekerUserId,
        int contactRequestId,
        RespondContactRequestDto request)
    {
        if (request.Status != ContactRequestStatus.Accepted &&
            request.Status != ContactRequestStatus.Declined)
        {
            throw new BadRequestException(
                "Contact request can only be accepted or declined.");
        }

        var jobSeekerProfile =
            await _jobSeekerRepository.GetByUserIdAsync(jobSeekerUserId)
            ?? throw new NotFoundException(
                "Job seeker profile was not found.");

        var contactRequest =
            await _contactRequestRepository.GetByIdWithDetailsAsync(
                contactRequestId)
            ?? throw new NotFoundException(
                "Contact request was not found.");

        if (contactRequest.JobSeekerProfileId != jobSeekerProfile.Id)
        {
            throw new ForbiddenException(
                "You cannot respond to this contact request.");
        }

        if (contactRequest.Status != ContactRequestStatus.Pending)
        {
            throw new BadRequestException(
                "This contact request has already been responded to.");
        }

        contactRequest.Status = request.Status;
        contactRequest.RespondedAt = DateTime.UtcNow;

        await _contactRequestRepository.SaveChangesAsync();

        var employerUserId =
            contactRequest.EmployerProfile.UserId;

        var notificationType =
            request.Status == ContactRequestStatus.Accepted
                ? NotificationType.ContactRequestAccepted
                : NotificationType.ContactRequestDeclined;

        var message =
            request.Status == ContactRequestStatus.Accepted
                ? "Your contact request has been accepted."
                : "Your contact request has been declined.";

        await _notificationService.CreateAsync(
            employerUserId,
            notificationType,
            "Contact request response",
            message,
            contactRequest.Id);

        return MapResponse(contactRequest);
    }

    public async Task<ContactDetailsResponseDto> GetContactDetailsAsync(
        int employerUserId,
        int contactRequestId)
    {
        var employerProfile =
            await _employerProfileRepository.GetByUserIdAsync(employerUserId)
            ?? throw new NotFoundException(
                "Employer profile was not found.");

        var contactRequest =
            await _contactRequestRepository.GetByIdWithDetailsAsync(
                contactRequestId)
            ?? throw new NotFoundException(
                "Contact request was not found.");

        if (contactRequest.EmployerProfileId != employerProfile.Id)
        {
            throw new ForbiddenException(
                "You cannot view this contact request.");
        }

        if (contactRequest.Status != ContactRequestStatus.Accepted)
        {
            throw new ForbiddenException(
                "Contact details are available only after the request is accepted.");
        }

        return new ContactDetailsResponseDto
        {
            ContactRequestId = contactRequest.Id,
            JobSeekerName =
                contactRequest.JobSeekerProfile.User.FullName,
            Email =
                contactRequest.JobSeekerProfile.User.Email,
            Phone =
                contactRequest.JobSeekerProfile.Phone
        };
    }

    private static ContactRequestResponseDto MapResponse(
        ContactRequest contactRequest)
    {
        return new ContactRequestResponseDto
        {
            Id = contactRequest.Id,
            JobApplicationId = contactRequest.JobApplicationId,
            EmployerProfileId = contactRequest.EmployerProfileId,
            JobSeekerProfileId = contactRequest.JobSeekerProfileId,
            Status = contactRequest.Status.ToString(),
            CreatedAt = contactRequest.CreatedAt,
            RespondedAt = contactRequest.RespondedAt,
            VacancyTitle = contactRequest.JobApplication?.Vacancy?.Title ?? string.Empty,
            CompanyName = contactRequest.EmployerProfile?.CompanyName ?? string.Empty,
            JobSeekerName = contactRequest.JobSeekerProfile?.User?.FullName ?? string.Empty
        };
    }
}