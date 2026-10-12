using CareerPulse.Application.DTOs.Interviews;
using MediatR;

namespace CareerPulse.Application.Features.Interviews.Queries.GetInterviewById;

/// <summary>
/// Query to retreive a single Interview by ID.
/// </summary>
public sealed record GetInterviewByIdQuery(Guid ApplicationId, Guid InterviewId) : IRequest<InterviewDto?>;
