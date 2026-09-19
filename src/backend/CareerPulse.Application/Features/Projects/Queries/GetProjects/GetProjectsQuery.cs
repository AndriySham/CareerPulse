using CareerPulse.Application.DTOs.Projects;
using MediatR;

namespace CareerPulse.Application.Features.Projects.Queries.GetProjects;

/// <summary>
/// Query to retrieve projects for a specific resume revision.
/// </summary>
public sealed record class GetProjectsQuery(Guid ResumeRevisionId) : IRequest<List<ProjectDto>>;
