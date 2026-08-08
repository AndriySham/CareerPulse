using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Applications;

/// <summary>
/// DTO for transitioning an Application to a new status in the Kanban pipeline.
/// </summary>
public sealed class ChangeApplicationStatusDto
{
    public ApplicationStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}
