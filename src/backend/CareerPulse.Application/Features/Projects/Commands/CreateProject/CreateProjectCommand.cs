using CareerPulse.Application.DTOs.Projects;
using MediatR;

namespace CareerPulse.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Command to create a new Project.
/// </summary>
public sealed record CreateProjectCommand(CreateProjectDto Dto) : IRequest<ProjectDto>;
