using CareerPulse.Application.DTOs.Recruiters;
using MediatR;

namespace CareerPulse.Application.Features.Recruiters.Commands;

/// <summary>
/// Command to create a new Recruiter.
/// </summary>
public sealed record CreateRecruiterCommand(Guid CompanyId, CreateRecruiterDto Dto) 
    : IRequest<RecruiterDto>;
