using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.StateMachines;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Aggregate Root: Central domain anchor of CareerPulse.
/// Links Company, Vacancy, and a specific ResumeRevision.
/// Controls the ApplicationStatus state machine (ADR 005).
/// </summary>
public sealed class Application
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? VacancyId { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public DateTime? SubmissionDate { get; private set; }
    public string JobSource { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties — EF Core only, private setters preserve encapsulation
    public Company Company { get; private set; } = null!;
    public Vacancy? Vacancy { get; private set; }
    public ResumeRevision ResumeRevision { get; private set; } = null!;

    private readonly List<Interview> _interviews = new();
    public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();

    // Required for EF Core
    private Application() { }

    public static Application Create(
        Guid companyId,
        Guid resumeRevisionId,
        string jobSource,
        Guid? vacancyId = null)
    {
        if (string.IsNullOrWhiteSpace(jobSource))
            throw new DomainException("JobSource is required.");

        return new Application
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            VacancyId = vacancyId,
            ResumeRevisionId = resumeRevisionId,
            Status = ApplicationStatus.Draft,
            JobSource = jobSource.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Transitions this Application to a new status.
    /// ADR 005: Transitioning to Applied locks the linked ResumeRevision.
    /// The ApplicationService layer is responsible for calling ResumeRevision.MarkAsApplied().
    /// </summary>
    public void TransitionTo(ApplicationStatus newStatus)
    {
        ApplicationStatusMachine.ValidateTransition(Status, newStatus);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == ApplicationStatus.Applied)
            SubmissionDate = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public IReadOnlySet<ApplicationStatus> GetAllowedTransitions()
        => ApplicationStatusMachine.GetAllowedTransitions(Status);
}
