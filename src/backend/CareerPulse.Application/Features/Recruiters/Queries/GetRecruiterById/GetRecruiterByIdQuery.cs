using CareerPulse.Application.DTOs.Recruiters;
using MediatR;

namespace CareerPulse.Application.Features.Recruiters.Queries.GetRecruiterById;

/// <summary>
/// Query to retrieve a single Recruiter by ID.
/// </summary>
public sealed record GetRecruiterByIdQuery(Guid CompanyId, Guid RecruiterId) : IRequest<RecruiterDto?>;
