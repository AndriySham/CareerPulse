using CareerPulse.Application.DTOs.Interviews;
using MediatR;

namespace CareerPulse.Application.Features.Interviews.Commands.CreateInterview;

/// <summary>
/// Command to create a new Interview.
/// </summary>
public sealed record CreateInterviewCommand(Guid ApplicationId, CreateInterviewDto Dto) : IRequest<InterviewDto>;
