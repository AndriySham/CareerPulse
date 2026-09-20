using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Command handler for updating a Project entity.
/// </summary>
public sealed class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if(project == null)
        {
            throw new ResourceNotFoundException($"Project with ID '{request.Id}' was not found.");
        }

        var isResisionUsed = await _context.Applications
            .AnyAsync(a => a.ResumeRevisionId == project.ResumeRevisionId, cancellationToken);
        if(isResisionUsed)
        {
            throw new ConflictException("Project cannot be updated because its resume revision is already used in an application.");
        }

        project.Update(
            request.Dto.ProjectName,
            request.Dto.Description,
            request.Dto.Role,
            request.Dto.RepositoryUrl,
            request.Dto.LiveDemoUrl,
            request.Dto.TechStack);

        await _context.SaveChangesAsync(cancellationToken);

        return ProjectMapping.MapToDto(project);
    }
}
