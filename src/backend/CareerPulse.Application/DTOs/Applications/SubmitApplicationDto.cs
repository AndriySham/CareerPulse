namespace CareerPulse.Application.DTOs.Applications;

/// <summary>
/// DTO for creating and submitting a new Application.
/// </summary>
public sealed class SubmitApplicationDto
{
    public Guid CompanyId { get; init; }
    public Guid ResumeRevisionId { get; init; }
    public Guid? VacancyId { get; init; }
    public string JobSource { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public bool SubmitImmediately { get; init; } = true;
}
