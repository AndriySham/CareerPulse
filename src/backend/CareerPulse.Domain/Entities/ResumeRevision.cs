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
/// ADR 010 — Resume Profile and Revision Architecture.
/// </summary>
public sealed class ResumeRevision
{
    public Guid Id { get; private set; }
    public Guid ResumeId { get; private set; }
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

    private readonly List<WorkExperience> _workExperiences = new();
    public IReadOnlyCollection<WorkExperience> WorkExperiences => _workExperiences.AsReadOnly();

    private readonly List<Education> _educations = new();
    public IReadOnlyCollection<Education> Educations => _educations.AsReadOnly();

    private readonly List<Project> _projects = new();
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

    private readonly List<Language> _languages = new();
    public IReadOnlyCollection<Language> Languages => _languages.AsReadOnly();

    // Required for EF Core
    private ResumeRevision() { }

    public static ResumeRevision CreateDraft(Guid resumeId, PersonalInfo personalInfo, string professionalSummary)
    {
        if (resumeId == Guid.Empty)
        {
            throw new DomainException("ResumeId is required for ResumeRevision.");
        }

        return new ResumeRevision
        {
            Id = Guid.NewGuid(),
            ResumeId = resumeId,
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
    /// Deep-copies all owned snapshot collections.
    /// </summary>
    public ResumeRevision SpawnNewVersion()
    {
        if (Status != RevisionStatus.Applied)
        {
            throw new DomainException("Only an Applied ResumeRevision can spawn a new version.");
        }

        var copy = new ResumeRevision
        {
            Id = Guid.NewGuid(),
            ResumeId = ResumeId,
            Status = RevisionStatus.Draft,
            PersonalInfo = PersonalInfo,
            ProfessionalSummary = ProfessionalSummary,
            FileReference = null, // PDF attachment is not cloned
            Version = Version + 1,
            ParentRevisionId = Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var skill in _skills)
        {
            copy._skills.Add(skill.DeepCopy(copy.Id));
        }

        foreach (var work in _workExperiences)
        {
            copy._workExperiences.Add(work.DeepCopy(copy.Id));
        }

        foreach (var edu in _educations)
        {
            copy._educations.Add(edu.DeepCopy(copy.Id));
        }

        foreach (var proj in _projects)
        {
            copy._projects.Add(proj.DeepCopy(copy.Id));
        }

        foreach (var lang in _languages)
        {
            copy._languages.Add(lang.DeepCopy(copy.Id));
        }

        return copy;
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

    public void AddWorkExperience(WorkExperience experience)
    {
        EnsureDraft();
        _workExperiences.Add(experience);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveWorkExperience(Guid experienceId)
    {
        EnsureDraft();
        var item = _workExperiences.FirstOrDefault(w => w.Id == experienceId)
            ?? throw new DomainException($"WorkExperience {experienceId} not found in this revision.");
        _workExperiences.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEducation(Education education)
    {
        EnsureDraft();
        _educations.Add(education);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveEducation(Guid educationId)
    {
        EnsureDraft();
        var item = _educations.FirstOrDefault(e => e.Id == educationId)
            ?? throw new DomainException($"Education {educationId} not found in this revision.");
        _educations.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddProject(Project project)
    {
        EnsureDraft();
        _projects.Add(project);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveProject(Guid projectId)
    {
        EnsureDraft();
        var item = _projects.FirstOrDefault(p => p.Id == projectId)
            ?? throw new DomainException($"Project {projectId} not found in this revision.");
        _projects.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLanguage(Language language)
    {
        EnsureDraft();
        _languages.Add(language);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLanguage(Guid languageId)
    {
        EnsureDraft();
        var item = _languages.FirstOrDefault(l => l.Id == languageId)
            ?? throw new DomainException($"Language {languageId} not found in this revision.");
        _languages.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureDraft()
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException(
                "Cannot modify an Applied (Read-Only) ResumeRevision. Use SpawnNewVersion() to create an editable copy.");
    }
}
