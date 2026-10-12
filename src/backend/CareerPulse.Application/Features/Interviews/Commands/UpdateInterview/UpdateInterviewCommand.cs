using CareerPulse.Application.DTOs.Interviews;
using MediatR;

namespace CareerPulse.Application.Features.Interviews.Commands.UpdateInterview;

/// <summary>
/// Command to update an Interview.
/// </summary>
public sealed record UpdateInterviewCommand(Guid ApplicationId, Guid InterviewId, UpdateInterviewDto Dto)
    : IRequest<InterviewDto>;
