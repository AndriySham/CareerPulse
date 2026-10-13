using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.CreateApplicationCommunication;

/// <summary>
/// Command handler to create a new ApplicationCommunication entity.
/// </summary>
public sealed class CreateApplicationCommunicationCommandHandler
    : IRequestHandler<CreateApplicationCommunicationCommand, ApplicationCommunicationDto>
{
    private readonly IApplicationDbContext _context;

    public CreateApplicationCommunicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationCommunicationDto> Handle(
        CreateApplicationCommunicationCommand request,
        CancellationToken cancellationToken)
    {
        var applicationExists = await _context.Applications
            .AnyAsync(x => x.Id == request.ApplicationId, cancellationToken);
        if (!applicationExists)
        {
            throw new ResourceNotFoundException($"Application with ID '{request.ApplicationId}' was not found.");
        }

        var communication = ApplicationCommunication.Create(
            request.ApplicationId,
            request.Dto.OccurredAt,
            request.Dto.Notes);

        _context.ApplicationCommunications.Add(communication);
        await _context.SaveChangesAsync(cancellationToken);

        return ApplicationCommunicationMapping.MapToDto(communication);
    }
}
