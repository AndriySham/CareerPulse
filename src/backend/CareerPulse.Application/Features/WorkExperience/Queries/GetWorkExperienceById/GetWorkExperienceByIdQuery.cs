using CareerPulse.Application.DTOs.WorkExperiences;
using MediatR;

namespace CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperienceById;

/// <summary>
///  Query to retreive a single WorkExperience by ID.
/// </summary>
public sealed record GetWorkExperienceByIdQuery(Guid Id) : IRequest<WorkExperienceDto?>;
