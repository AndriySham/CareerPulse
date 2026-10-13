using CareerPulse.Application.DTOs.ApplicationCommunications;
using MediatR;

namespace CareerPulse.Application.Features.ApplicationCommunications.Queries.GetApplicationCommunicationById;

/// <summary>
/// Query to retreive a single ApplicationCommunication by ID.
/// </summary>
public sealed record GetApplicationCommunicationByIdQuery(Guid ApplicationId, Guid ApplicationCommunicationId) 
    : IRequest<ApplicationCommunicationDto?>;
