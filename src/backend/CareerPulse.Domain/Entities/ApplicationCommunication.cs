namespace CareerPulse.Domain.Entities;

/// <summary>
/// Represents a single ApplicationCommunication round associated with an Application.
/// Child entity of the Application aggregate.
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
}
