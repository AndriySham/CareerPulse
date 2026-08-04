namespace CareerPulse.Domain.Enums;

/// <summary>
/// Status of a ResumeRevision snapshot.
/// ADR 005: Draft is editable; Applied is immutable (Read-Only).
/// </summary>
public enum RevisionStatus
{
    Draft,
    Applied // Terminal — Read-Only once linked to an Application
}
