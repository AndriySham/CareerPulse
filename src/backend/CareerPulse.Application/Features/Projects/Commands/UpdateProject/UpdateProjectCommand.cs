using CareerPulse.Application.DTOs.Projects;
using MediatR;

namespace CareerPulse.Application.Features.Projects;

/// <summary>
/// Command to update a new Project.
/// </summary>
public sealed record UpdateProjectCommand(Guid Id, UpdateProjectDto Dto) : IRequest<ProjectDto>;
