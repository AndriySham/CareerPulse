using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Projects.Queries.GetProjects;

/// <summary>
/// Query handler for retrieving projects with AsNoTracking().
/// </summary>
public sealed class GetProjectsQueryHandler
    : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectDto>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.ResumeRevisionId == request.ResumeRevisionId)
            .ToListAsync(cancellationToken);

        return projects
            .Select(ProjectMapping.MapToDto)
            .ToList();
    }
}
