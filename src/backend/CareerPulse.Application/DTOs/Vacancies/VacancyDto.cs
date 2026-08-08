namespace CareerPulse.Application.DTOs.Vacancies;

/// <summary>
/// DTO representing a Vacancy entity.
/// </summary>
public sealed class VacancyDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Url { get; init; }
    public DateTime? PostedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
