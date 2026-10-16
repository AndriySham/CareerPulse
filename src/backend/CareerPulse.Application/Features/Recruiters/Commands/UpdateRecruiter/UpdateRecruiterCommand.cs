using CareerPulse.Application.DTOs.Recruiters;
using MediatR;

namespace CareerPulse.Application.Features.Recruiters.Commands.UpdateRecruiter;

/// <summary>
/// Command to update a Recruiter.
/// </summary>
public sealed record UpdateRecruiterCommand(Guid CompanyId, Guid RecruiterId, UpdateRecruiterDto Dto)
    : IRequest<RecruiterDto>;
