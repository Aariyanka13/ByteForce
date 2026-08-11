using SmartRecruitmentMatchingPlatform.DTOs.ContactRequests;

namespace SmartRecruitmentMatchingPlatform.Interface.Services;

public interface IContactRequestService
{
    Task<ContactRequestResponseDto> CreateAsync(
        int employerUserId,
        CreateContactRequestDto request);

    Task<List<ContactRequestResponseDto>> GetEmployerRequestsAsync(
        int employerUserId);

    Task<List<ContactRequestResponseDto>> GetJobSeekerRequestsAsync(
        int jobSeekerUserId);

    Task<ContactRequestResponseDto> RespondAsync(
        int jobSeekerUserId,
        int contactRequestId,
        RespondContactRequestDto request);

    Task<ContactDetailsResponseDto> GetContactDetailsAsync(
        int employerUserId,
        int contactRequestId);
}