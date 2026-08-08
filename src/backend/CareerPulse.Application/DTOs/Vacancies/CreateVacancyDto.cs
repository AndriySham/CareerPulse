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
    public DateTime? PostedAt { get; init; }
}
