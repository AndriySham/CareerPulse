using CareerPulse.Application.DTOs.Resumes;
using MediatR;

namespace CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisionById;

/// <summary>
/// Query to retrieve a single ResumeRevision by ID with AsNoTracking().
/// </summary>
public sealed record GetResumeRevisionByIdQuery(Guid Id) : IRequest<ResumeRevisionDto?>;
