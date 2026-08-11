using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string ProjectName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Role { get; private set; }
    public string? RepositoryUrl { get; private set; }
    public string? LiveDemoUrl { get; private set; }
    public string? TechStack { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Project() { }

    public static Project Create(
        Guid resumeRevisionId,
        string projectName,
        string? description = null,
        string? role = null,
        string? repositoryUrl = null,
        string? liveDemoUrl = null,
        string? techStack = null)
    {
        if (string.IsNullOrWhiteSpace(projectName))
            throw new DomainException("ProjectName is required.");

        return new Project
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = resumeRevisionId,
            ProjectName = projectName.Trim(),
            Description = description,
            Role = role,
            RepositoryUrl = repositoryUrl,
            LiveDemoUrl = liveDemoUrl,
            TechStack = techStack,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal Project DeepCopy(Guid newRevisionId)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = newRevisionId,
            ProjectName = ProjectName,
            Description = Description,
            Role = Role,
            RepositoryUrl = RepositoryUrl,
            LiveDemoUrl = LiveDemoUrl,
            TechStack = TechStack,
            CreatedAt = DateTime.UtcNow
        };
    }
}
