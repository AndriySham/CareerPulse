using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class ProjectMapping
{
    public static ProjectDto MapToDto(Project project) => new()
    {
        Id = project.Id,
        ResumeRevisionId = project.ResumeRevisionId,
        Role = project.Role,
        ProjectName = project.ProjectName,
        Description = project.Description,
        TechStack = project.TechStack,
        RepositoryUrl = project.RepositoryUrl,
        LiveDemoUrl = project.LiveDemoUrl,
        CreatedAt = project.CreatedAt
    };
}

