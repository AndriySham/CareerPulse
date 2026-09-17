using MediatR;

namespace CareerPulse.Application.Features.Educations.Commands.DeleteEducation;

/// <summary>
/// Command to delete an Education.
/// </summary>
public sealed record DeleteEducationCommand(Guid Id) : IRequest;
