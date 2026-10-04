using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Applications;

/// <summary>
/// DTO representing an Application details in Application page.
/// </summary>
public sealed class ApplicationDetailsDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public Guid? VacancyId { get; init; }
    public string? VacancyTitle { get; init; }
    public Guid ResumeRevisionId { get; init; }
    public ApplicationStatus Status { get; init; }
    public string JobSource { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime? AppliedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyCollection<ApplicationStatus> AllowedTransitions { get; init; } = Array.Empty<ApplicationStatus>();
    public IReadOnlyCollection<ApplicationCommunicationDto> Communications { get; init; } = Array.Empty<ApplicationCommunicationDto>();
    public IReadOnlyCollection<InterviewDto> Interviews { get; init; } = Array.Empty<InterviewDto>();
    public RecruiterDto? Recruiter { get; init; }
}
