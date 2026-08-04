using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.StateMachines;

/// <summary>
/// Enforces valid ApplicationStatus transitions.
/// ADR 005: Transition rules are domain invariants — not infrastructure concerns.
/// </summary>
public static class ApplicationStatusMachine
{
    private static readonly IReadOnlyDictionary<ApplicationStatus, HashSet<ApplicationStatus>> ValidTransitions =
        new Dictionary<ApplicationStatus, HashSet<ApplicationStatus>>
        {
            [ApplicationStatus.Draft]              = [ApplicationStatus.Applied],
            [ApplicationStatus.Applied]            = [ApplicationStatus.Viewed, ApplicationStatus.Rejected, ApplicationStatus.NoResponse],
            [ApplicationStatus.Viewed]             = [ApplicationStatus.HRInterview, ApplicationStatus.Rejected],
            [ApplicationStatus.HRInterview]        = [ApplicationStatus.TechnicalInterview, ApplicationStatus.Rejected],
            [ApplicationStatus.TechnicalInterview] = [ApplicationStatus.Offer, ApplicationStatus.Rejected],
            [ApplicationStatus.Offer]              = [],
            [ApplicationStatus.Rejected]           = [],
            [ApplicationStatus.NoResponse]         = [],
        };

    /// <summary>
    /// Validates that a transition from <paramref name="current"/> to <paramref name="next"/> is allowed.
    /// Throws <see cref="DomainException"/> if the transition is invalid.
    /// </summary>
    public static void ValidateTransition(ApplicationStatus current, ApplicationStatus next)
    {
        if (!ValidTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next))
        {
            var allowedList = ValidTransitions.TryGetValue(current, out var set) && set.Count > 0
                ? string.Join(", ", set)
                : "none (terminal state)";

            throw new DomainException(
                $"Invalid status transition: {current} → {next}. " +
                $"Allowed transitions from {current}: [{allowedList}].");
        }
    }

    /// <summary>Returns the set of valid next statuses from the current state.</summary>
    public static IReadOnlySet<ApplicationStatus> GetAllowedTransitions(ApplicationStatus current)
        => ValidTransitions.TryGetValue(current, out var set)
            ? set
            : new HashSet<ApplicationStatus>();
}
