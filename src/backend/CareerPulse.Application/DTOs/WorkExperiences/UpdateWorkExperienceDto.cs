namespace CareerPulse.Application.DTOs.WorkExperiences;

/// <summary>
/// Input DTO for updating a WorkExperience. 
/// </summary>
public sealed class UpdateWorkExperienceDto
{
    public string CompanyName { get; init; } = string.Empty;
    public string PositionTitle { get; init; } = string.Empty;
    public int StartMonth { get; init; }
    public int StartYear { get; init; }
    public int? EndMonth { get; init; }
    public int? EndYear { get; init; }
    public bool IsCurrentJob { get; init; }
    public string? Description { get; init; }
    public string? Achievements { get; init; }
    public string? TechStack { get; init; }
}
