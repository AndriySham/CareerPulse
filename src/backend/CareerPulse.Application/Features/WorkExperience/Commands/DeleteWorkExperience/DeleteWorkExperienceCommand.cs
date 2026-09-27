using MediatR;

namespace CareerPulse.Application.Features.WorkExperience.Commands.DeleteWorkExperience;

/// <summary>
/// Command to delete a WorkExperience.
/// </summary>
public sealed record DeleteWorkExperienceCommand(Guid Id) : IRequest;
