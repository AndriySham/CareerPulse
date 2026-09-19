using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Query handler for retrieving a single Project by ID with AsNoTracking().
/// </summary>
public sealed class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProjectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto?> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return project is null ? null : ProjectMapping.MapToDto(project);
    }
}
