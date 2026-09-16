namespace CareerPulse.Application.DTOs.Educations;

/// <summary>
/// DTO representing an Education entity.
/// </summary>
public sealed class EducationDto
{
    public Guid Id { get; init; }
    public Guid ResumeRevisionId { get; init; }
    public string InstitutionName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
}
