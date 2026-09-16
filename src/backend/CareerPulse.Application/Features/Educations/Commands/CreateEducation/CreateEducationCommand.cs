using CareerPulse.Application.DTOs.Educations;
using MediatR;

namespace CareerPulse.Application.Features.Educations.Commands.CreateEducation;

/// <summary>
/// Command to create a new Education.
/// </summary>
public sealed record CreateEducationCommand(CreateEducationDto Dto) : IRequest<EducationDto>;
