using CareerPulse.Domain.Enums;

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
    public DateTime ScheduledAt { get; private set; }
    public string? Notes { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Interview() { }

    public static Interview Schedule(Guid applicationId, InterviewType type, DateTime scheduledAt) => new()
    {
        Id = Guid.NewGuid(),
        ApplicationId = applicationId,
        Type = type,
        ScheduledAt = scheduledAt,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

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
