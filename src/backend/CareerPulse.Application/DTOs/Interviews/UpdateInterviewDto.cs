using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Interviews;

/// <summary>
/// Input DTO for updating an existing Interview.
/// </summary>
public sealed class UpdateInterviewDto
{
    public InterviewType Type { get; init; }
    public DateTime? ScheduledAt { get; init; }
    public DateTime? ConductedAt { get; init; }
    public string? Notes { get; init; }
    public string? Feedback { get; init; }
}
