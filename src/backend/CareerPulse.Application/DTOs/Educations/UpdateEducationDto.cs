namespace CareerPulse.Application.DTOs.Educations;

/// <summary>
/// Input DTO for updating an Education.
/// </summary>
public sealed class UpdateEducationDto
{
    public Guid ResumeRevisionId { get; init; }
    public string InstitutionName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
}
