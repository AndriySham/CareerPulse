using MediatR;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.DeleteApplicationCommunication;

/// <summary>
/// Command to delete an ApplicationCommunication.
/// </summary>
public sealed record DeleteApplicationCommunicationCommand(Guid ApplicationId, Guid ApplicationCommunicationId) : IRequest;
