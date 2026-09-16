namespace CareerPulse.Application.DTOs.Educations;

/// <summary>
/// Input DTO for creating a new Education.
/// </summary>
public sealed class CreateEducationDto
{
    public Guid ResumeRevisionId { get; init; }
    public string InstitutionName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
}
