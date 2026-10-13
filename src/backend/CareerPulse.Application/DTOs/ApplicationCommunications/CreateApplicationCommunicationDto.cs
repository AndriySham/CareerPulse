namespace CareerPulse.Application.DTOs.ApplicationCommunications;

/// <summary>
/// Input DTO for creating a new ApplicationCommunication.
/// </summary>
public sealed class CreateApplicationCommunicationDto
{
    public DateTime OccurredAt { get; init; }
    public string Notes { get; init; } = string.Empty;
}
