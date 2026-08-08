using CareerPulse.Application.DTOs.Applications;
using MediatR;

namespace CareerPulse.Application.Features.Applications.Queries.GetApplicationById;

/// <summary>
/// Query to retrieve a single application by ID.
/// </summary>
public sealed record GetApplicationByIdQuery(Guid Id) : IRequest<ApplicationDto?>;
