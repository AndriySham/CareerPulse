using CareerPulse.Application.DTOs.WorkExperiences;
using MediatR;

namespace CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperiences;

/// <summary>
/// Query to retrieve  workExperiences for a specific resume revision.
/// </summary>
public sealed record GetWorkExperiencesQuery(Guid ResumeRevisionId)
    : IRequest<List<WorkExperienceDto>>;
