using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Educations.Commands.DeleteEducation;

/// <summary>
/// Command handler for delete an existing Education entity.
/// </summary>
public sealed class DeleteEducationCommandHandler
    : IRequestHandler<DeleteEducationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteEducationCommand request,
        CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (education == null)
        {
            throw new ResourceNotFoundException($"Education with ID '{request.Id}' was not found.");
        }

        _context.Educations.Remove(education);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
