using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.UpdateApplicationCommunication;

/// <summary>
/// Command handler for updating an existing ApplicationCommunication entity.
/// </summary>
public sealed class UpdateApplicationCommunicationCommandHandler
    : IRequestHandler<UpdateApplicationCommunicationCommand, ApplicationCommunicationDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateApplicationCommunicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationCommunicationDto> Handle(
        UpdateApplicationCommunicationCommand request,
        CancellationToken cancellationToken)
    {
        var communication = await _context.ApplicationCommunications
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationCommunicationId &&
                x.ApplicationId == request.ApplicationId, cancellationToken);
        if (communication == null)
        {
            throw new ResourceNotFoundException($"ApplicationCommunication with application ID '{request.ApplicationId}' " +
                $"and applicationCommunication ID '{request.ApplicationCommunicationId}' was not found.");
        }

        communication.Update(request.Dto.OccurredAt, request.Dto.Notes);
        await _context.SaveChangesAsync(cancellationToken);

        return ApplicationCommunicationMapping.MapToDto(communication);
    }
}
