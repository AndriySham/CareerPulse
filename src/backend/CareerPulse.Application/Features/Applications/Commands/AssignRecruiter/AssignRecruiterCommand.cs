using CareerPulse.Application.DTOs.Applications;
using MediatR;

namespace CareerPulse.Application.Features.Applications.Commands.AssignRecruiter;

/// <summary>
/// Command to assign or unassign a Recruiter for an existing Application.
/// </summary>
public sealed record AssignRecruiterCommand(Guid ApplicationId, Guid? RecruiterId) : IRequest<ApplicationDetailsDto>;
