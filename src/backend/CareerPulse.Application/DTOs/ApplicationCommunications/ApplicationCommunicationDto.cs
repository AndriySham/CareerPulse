namespace CareerPulse.Application.DTOs.ApplicationCommunications;

/// <summary>
/// DTO representing an ApplicationCommunication Entity.
/// </summary>
public sealed class ApplicationCommunicationDto
{
    public Guid Id { get; init; }
    public DateTime OccurredAt { get; init; }
    public string Notes { get; init; } = string.Empty;
}
