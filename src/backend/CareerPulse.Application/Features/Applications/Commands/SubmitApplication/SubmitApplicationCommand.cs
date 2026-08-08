using CareerPulse.Application.DTOs.Applications;
using MediatR;

namespace CareerPulse.Application.Features.Applications.Commands.SubmitApplication;

/// <summary>
/// Command to create and optionally submit a new Application.
/// </summary>
public sealed record SubmitApplicationCommand(SubmitApplicationDto Dto) : IRequest<ApplicationDto>;
