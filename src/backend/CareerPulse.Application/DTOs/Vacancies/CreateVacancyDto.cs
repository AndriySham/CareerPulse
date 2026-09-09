using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Vacancies;

/// <summary>
/// Input DTO for creating a new Vacancy.
/// </summary>
public sealed class CreateVacancyDto
{
    public Guid CompanyId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Url { get; init; }
    public string? Location { get; init; }
    public WorkMode? WorkMode { get; init; }
    public int? SalaryMin { get; init; }
    public int? SalaryMax { get; init; }
    public SalaryCurrency? SalaryCurrency { get; init; }
    public DateTime? PostedAt { get; init; }
}
