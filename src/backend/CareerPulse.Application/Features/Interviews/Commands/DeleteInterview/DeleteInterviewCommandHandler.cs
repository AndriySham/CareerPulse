using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Interviews.Commands.DeleteInterview;

/// <summary>
/// Command handler for deleting an existing Interview Entity.
/// </summary>
public sealed class DeleteInterviewCommandHandler : IRequestHandler<DeleteInterviewCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteInterviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteInterviewCommand request,
        CancellationToken cancellationToken)
    {
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(x => x.Id == request.InterviewId && 
                x.ApplicationId == request.ApplicationId, cancellationToken);
        if (interview == null)
        {
            throw new ResourceNotFoundException($"Resource with application ID '{request.ApplicationId}' and interview ID '{request.InterviewId}' was not found.");
        }

        _context.Interviews.Remove(interview);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

