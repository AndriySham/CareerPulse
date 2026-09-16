using CareerPulse.Application.DTOs.Educations;
using MediatR;

namespace CareerPulse.Application.Features.Educations.Queries.GetEducations;

/// <summary>
/// Query to retrieve educations for a specific resume revision.
/// </summary>
public sealed record GetEducationsQuery(Guid ResumeRevisionId) 
    : IRequest<List<EducationDto>>;
