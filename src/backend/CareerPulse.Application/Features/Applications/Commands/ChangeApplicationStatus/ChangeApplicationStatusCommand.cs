using CareerPulse.Application.DTOs.Applications;
using MediatR;

namespace CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;

/// <summary>
/// Command to change an Application's status in the Kanban pipeline.
/// </summary>
public sealed record ChangeApplicationStatusCommand(Guid Id, ChangeApplicationStatusDto Dto) : IRequest<ApplicationDto>;
