using CareerPulse.Application.DTOs.WorkExperiences;
using MediatR;

namespace CareerPulse.Application.Features.WorkExperience.Commands.UpdateWorkExperience;

/// <summary>
/// Command to update a WorkExperience.
/// </summary>
public sealed record UpdateWorkExperienceCommand(Guid Id, UpdateWorkExperienceDto Dto)
    : IRequest<WorkExperienceDto>;
