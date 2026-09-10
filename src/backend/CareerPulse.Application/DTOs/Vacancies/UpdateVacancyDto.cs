using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Vacancies;

/// <summary>
/// Input DTO for updating an existing Vacancy.
/// </summary>
public sealed class UpdateVacancyDto
{
    public string Title { get; init; } = string.Empty;
    public string? Location { get; init; }
    public WorkMode? WorkMode { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public int? SalaryMin { get; init; }
    public int? SalaryMax { get; init; }
    public SalaryCurrency? SalaryCurrency { get; init; }
    public string? Description { get; init; }
    public string? Responsibilities { get; init; }
    public string? Requirements { get; init; }
    public string? NiceToHave { get; init; }
    public string? Benefits { get; init; }
    public string? Url { get; init; }
}
