namespace CareerPulse.Application.DTOs.ApplicationCommunications;

/// <summary>
/// Input DTO for updating an existing ApplicationCommunication.
/// </summary>
public sealed class UpdateApplicationCommunicationDto
{
    public DateTime OccurredAt { get; init; }
    public string Notes { get; init; } = string.Empty;
}
