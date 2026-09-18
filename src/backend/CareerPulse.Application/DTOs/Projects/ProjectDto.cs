namespace CareerPulse.Application.DTOs.Projects;

/// <summary>
/// DTO representing a Project entity.
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid ResumeRevisionId { get; init; }
    public string? Role { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TechStack { get; init; }
    public string? RepositoryUrl { get; init; }
    public string? LiveDemoUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
