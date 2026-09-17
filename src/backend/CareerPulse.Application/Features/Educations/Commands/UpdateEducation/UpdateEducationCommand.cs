using CareerPulse.Application.DTOs.Educations;
using MediatR;

namespace CareerPulse.Application.Features.Educations.Commands.UpdateEducation;

/// <summary>
/// Command to update an Education.
/// </summary>
public sealed record UpdateEducationCommand(Guid Id, UpdateEducationDto Dto) 
    : IRequest<EducationDto>;
