using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.WorkExperience.Commands.DeleteWorkExperience;

/// <summary>
/// Command handler to delete an existing WorkExperience.
/// </summary>
public sealed class DeleteWorkExperienceCommandHandler
    : IRequestHandler<DeleteWorkExperienceCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteWorkExperienceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteWorkExperienceCommand request,
        CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if(workExperience == null)
        {
            throw new ResourceNotFoundException($"WorkExperience with ID '{request.Id}' was not found.");
        }

        var isRevisionUsed = await _context.Applications
            .AnyAsync(x => x.ResumeRevisionId == workExperience.ResumeRevisionId, cancellationToken);
        if(isRevisionUsed)
        {
            throw new ConflictException("WorkExperience cannot be deleted because its resume revision is already used in an application.");
        }

        _context.WorkExperiences.Remove(workExperience);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
