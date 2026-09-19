using CareerPulse.Application.DTOs.Projects;
using MediatR;

namespace CareerPulse.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Query to retrieve a single Project by ID.
/// </summary>
public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;
