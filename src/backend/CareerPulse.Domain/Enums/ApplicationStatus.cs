namespace CareerPulse.Domain.Enums;

/// <summary>
/// Represents the lifecycle state machine of a job application.
/// ADR 005: ApplicationStatus controls valid state transitions.
/// </summary>
public enum ApplicationStatus
{
    Draft,
    Applied,
    Viewed,
    HRInterview,
    TechnicalInterview,
    Offer,
    Rejected,
    NoResponse
}
