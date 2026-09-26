using CareerPulse.Application.DTOs.WorkExperiences;
using MediatR;

namespace CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;

/// <summary>
/// Command to create a new WorkExperience.
/// </summary>
public sealed record CreateWorkExperienceCommand(CreateWorkExperienceDto Dto) 
    : IRequest<WorkExperienceDto>;
