using MediatR;

namespace CareerPulse.Application.Features.Recruiters.Commands.DeleteRecruiter;

/// <summary>
/// Command to delete a Recruiter.
/// </summary>
public sealed record DeleteRecruiterCommand(Guid CompanyId, Guid RecruiterId) : IRequest;
