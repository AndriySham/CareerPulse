using MediatR;

namespace CareerPulse.Application.Features.Interviews.Commands.DeleteInterview;

/// <summary>
/// Command to delete an Interview.
/// </summary>
public sealed record DeleteInterviewCommand(Guid ApplicationId, Guid InterviewId) : IRequest;
