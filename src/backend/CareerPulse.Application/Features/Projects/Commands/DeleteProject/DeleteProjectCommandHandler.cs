using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Command handler for delete an existing Project entity.
/// </summary>
public sealed class DeleteProjectCommandHandler
    : IRequestHandler<DeleteProjectCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (project == null)
        {
            throw new ResourceNotFoundException($"Project with ID '{request.Id}' was not found.");
        }

        var isRevisionUsed = await _context.Applications
            .AnyAsync(x => x.ResumeRevisionId == project.ResumeRevisionId, cancellationToken);
        if (isRevisionUsed)
        {
            throw new ConflictException("Project cannot be deleted because its resume revision is already used in an application.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
