using CareerPulse.Application.DTOs.ApplicationCommunications;
using MediatR;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.CreateApplicationCommunication;

/// <summary>
/// Command to create a new ApplicationCommunication entity.
/// </summary>
public sealed record CreateApplicationCommunicationCommand(
    Guid ApplicationId,
    CreateApplicationCommunicationDto Dto)
    : IRequest<ApplicationCommunicationDto>;
