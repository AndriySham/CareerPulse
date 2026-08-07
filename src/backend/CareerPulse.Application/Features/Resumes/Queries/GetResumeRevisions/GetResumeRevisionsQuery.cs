using CareerPulse.Application.DTOs.Resumes;
using MediatR;

namespace CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisions;

/// <summary>
/// Query to retrieve ResumeRevisions (all or filtered by ID) with AsNoTracking().
/// </summary>
public sealed record GetResumeRevisionsQuery(Guid? Id = null) : IRequest<List<ResumeRevisionDto>>;
