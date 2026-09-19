using MediatR;

namespace CareerPulse.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Command to delete a Project.
/// </summary>
public sealed record DeleteProjectCommand(Guid Id) : IRequest;
