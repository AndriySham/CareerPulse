using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Represents a single interview round associated with an Application.
/// Child entity of the Application aggregate.
/// </summary>
public sealed class Interview
{
    public Guid Id { get; private set; }
    public Guid ApplicationId { get; private set; }
    public InterviewType Type { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? ConductedAt { get; private set; }
    public string? Notes { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Application Application { get; private set; } = null!;

    private Interview() { }

    public static Interview Create(
        Guid applicationId, 
        InterviewType type, 
        DateTime? scheduledAt, 
        DateTime? conductedAt)
    {
        if (scheduledAt is null && conductedAt is null)
            throw new DomainException("Interview must have either a scheduled or conducted date.");

        return new()
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Type = type,
            ScheduledAt = scheduledAt,
            ConductedAt = conductedAt,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void RecordFeedback(string feedback)
    {
        Feedback = feedback;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
