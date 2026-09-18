using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Command handler for creating a Project entity.
/// </summary>
public sealed class CreateProjectCommandHandler 
    : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var resumeRevisionExist = await _context.ResumeRevisions
            .AnyAsync(x => x.Id == request.Dto.ResumeRevisionId, cancellationToken);

        if (!resumeRevisionExist)
        {
            throw new ResourceNotFoundException($"ResumeRevision with ID '{request.Dto.ResumeRevisionId}' was not found");
        }

        var project = Project.Create(
            request.Dto.ResumeRevisionId,
            request.Dto.ProjectName,
            request.Dto.Description,
            request.Dto.Role,
            request.Dto.RepositoryUrl,
            request.Dto.LiveDemoUrl,
            request.Dto.TechStack);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return ProjectMapping.MapToDto(project);
    }
}
