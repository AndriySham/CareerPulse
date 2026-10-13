using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Represents a single ApplicationCommunication round associated with an Application.
/// References its Application by ApplicationId.
/// </summary>
public sealed class ApplicationCommunication
{
    public Guid Id { get; private set; }
    public Guid ApplicationId { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ApplicationCommunication() { }

    public static ApplicationCommunication Create(
        Guid applicationId,
        DateTime occuredAt,
        string notes)
    {
        if (applicationId == Guid.Empty)
            throw new DomainException("Application ID is required.");

        if (string.IsNullOrWhiteSpace(notes))
            throw new DomainException("Notes are required.");

        if (notes.Trim().Length > 4000)
            throw new DomainException("Notes must not exceed 4000 characters.");

        return new ApplicationCommunication
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            OccurredAt = occuredAt,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(DateTime occurredAt, string notes)
    {
        OccurredAt = occurredAt;
        Notes = notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    } 
}
