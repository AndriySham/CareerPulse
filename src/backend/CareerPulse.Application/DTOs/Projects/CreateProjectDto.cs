namespace CareerPulse.Application.DTOs.Projects;

/// <summary>
/// Input DTO for creating a new Project.
/// </summary>
public sealed class CreateProjectDto
{
    public Guid ResumeRevisionId { get; init; }
    public string? Role { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TechStack { get; init; }
    public string? RepositoryUrl { get; init; }
    public string? LiveDemoUrl { get; init; }
}
