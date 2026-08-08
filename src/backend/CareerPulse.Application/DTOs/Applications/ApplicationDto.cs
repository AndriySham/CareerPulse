using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Applications;

/// <summary>
/// DTO representing an Application entity in the Kanban pipeline and job search lifecycle.
/// </summary>
public sealed class ApplicationDto
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
}
