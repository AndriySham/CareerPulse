using CareerPulse.Application.DTOs.Educations;
using MediatR;

namespace CareerPulse.Application.Features.Educations.Queries.GetEducationById;

/// <summary>
/// Query to retrieve a single Education by ID.
/// </summary>
public sealed record GetEducationByIdQuery(Guid Id) : IRequest<EducationDto?>;
