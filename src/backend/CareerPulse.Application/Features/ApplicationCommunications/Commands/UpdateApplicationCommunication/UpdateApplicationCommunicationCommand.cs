using CareerPulse.Application.DTOs.ApplicationCommunications;
using MediatR;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.UpdateApplicationCommunication;

/// <summary>
/// Command to update an ApplicationCommunication.
/// </summary>
public sealed record UpdateApplicationCommunicationCommand(
    Guid ApplicationId,
    Guid ApplicationCommunicationId,
    UpdateApplicationCommunicationDto Dto)
    : IRequest<ApplicationCommunicationDto>;
