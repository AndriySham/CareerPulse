using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Immutable snapshot of structured resume data at a specific point in time.
/// ADR 005 — Draft Immutability: editable only in Draft status.
/// Once linked to an Application, becomes Read-Only.
/// Subsequent edits spawn a new version via SpawnNewVersion() (Copy-on-Write).
/// ADR 002 — PostgreSQL is SSOT; PDF is a FileReference attachment only.
/// </summary>
public sealed class ResumeRevision
{
    public Guid Id { get; private set; }
    public RevisionStatus Status { get; private set; }
    public PersonalInfo PersonalInfo { get; private set; } = null!;
    public string ProfessionalSummary { get; private set; } = string.Empty;
    public string? FileReference { get; private set; }  // IFileStorage key (ADR 003)
    public int Version { get; private set; }
    public Guid? ParentRevisionId { get; private set; } // Copy-on-Write chain
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ResumeRevisionSkill> _skills = new();
    public IReadOnlyCollection<ResumeRevisionSkill> Skills => _skills.AsReadOnly();

    // Required for EF Core
    private ResumeRevision() { }

    public static ResumeRevision CreateDraft(PersonalInfo personalInfo, string professionalSummary)
    {
        return new ResumeRevision
        {
            Id = Guid.NewGuid(),
            Status = RevisionStatus.Draft,
            PersonalInfo = personalInfo,
            ProfessionalSummary = professionalSummary,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a new Draft revision as a copy of this one (Copy-on-Write pattern).
    /// Called when user attempts to edit an Applied (Read-Only) revision.
    /// </summary>
    public ResumeRevision SpawnNewVersion()
    {
        return new ResumeRevision
        {
            Id = Guid.NewGuid(),
            Status = RevisionStatus.Draft,
            PersonalInfo = PersonalInfo,
            ProfessionalSummary = ProfessionalSummary,
            FileReference = null, // PDF attachment is not cloned
            Version = Version + 1,
            ParentRevisionId = Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Locks this revision as Read-Only.
    /// Called by ApplicationService when Application transitions to Applied.
    /// </summary>
    public void MarkAsApplied()
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException("ResumeRevision is already in Applied (Read-Only) state.");

        Status = RevisionStatus.Applied;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Sets the file storage reference. ADR 003 — IFileStorage key.</summary>
    public void SetFileReference(string fileReference)
    {
        EnsureDraft();
        FileReference = fileReference;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSummary(string summary)
    {
        EnsureDraft();
        ProfessionalSummary = summary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(PersonalInfo personalInfo)
    {
        EnsureDraft();
        PersonalInfo = personalInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>ADR 006 — Skills must resolve to MasterSkill before being added.</summary>
    public void AddSkill(Guid masterSkillId, int proficiencyLevel = 3)
    {
        EnsureDraft();
        if (_skills.Any(s => s.MasterSkillId == masterSkillId))
            throw new DomainException("Skill already exists in this revision.");

        _skills.Add(new ResumeRevisionSkill(Id, masterSkillId, proficiencyLevel));
    }

    public void RemoveSkill(Guid masterSkillId)
    {
        EnsureDraft();
        var skill = _skills.FirstOrDefault(s => s.MasterSkillId == masterSkillId)
            ?? throw new DomainException($"Skill {masterSkillId} not found in this revision.");
        _skills.Remove(skill);
    }

    private void EnsureDraft()
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException(
                "Cannot modify an Applied (Read-Only) ResumeRevision. Use SpawnNewVersion() to create an editable copy.");
    }
}
