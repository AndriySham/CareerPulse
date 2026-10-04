using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Interviews;

/// <summary>
/// DTO representing Interview entity.
/// </summary>
public sealed class InterviewDto
{
    public Guid Id { get; init; }
    public InterviewType Type { get; init; }
    public DateTime? ScheduledAt { get; init; }
    public DateTime? ConductedAt { get; init; }
    public string? Notes { get; init; }
    public string? Feedback { get; init; }
}
