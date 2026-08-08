namespace CareerPulse.Application.DTOs.Vacancies;

/// <summary>
/// Input DTO for updating an existing Vacancy.
/// </summary>
public sealed class UpdateVacancyDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Url { get; init; }
}
