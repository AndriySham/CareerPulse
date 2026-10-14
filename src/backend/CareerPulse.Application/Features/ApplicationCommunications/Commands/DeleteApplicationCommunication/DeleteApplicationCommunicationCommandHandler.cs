using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.DeleteApplicationCommunication;

/// <summary>
/// Command handler for deleting ApplicationCommunication entity.
/// </summary>
public sealed class DeleteApplicationCommunicationCommandHandler
    : IRequestHandler<DeleteApplicationCommunicationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteApplicationCommunicationCommandHandler(IApplicationDbContext contex)
    {
        _context = contex;
    }

    public async Task Handle(
        DeleteApplicationCommunicationCommand request,
        CancellationToken cancellationToken)
    {
        var communication = await _context.ApplicationCommunications
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationCommunicationId
                && x.ApplicationId == request.ApplicationId, cancellationToken);
        if (communication == null)
        {
            throw new ResourceNotFoundException($"ApplicationCommunication with application ID '{request.ApplicationId}' " +
                $"and communication ID '{request.ApplicationCommunicationId}' was not found.");
        }

        _context.ApplicationCommunications.Remove(communication);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
