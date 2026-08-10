### 3.2 Domain Entities

```csharp
namespace CareerPulse.Domain.Entities;

// Application (Central Anchor)
public sealed class Application
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? VacancyId { get; private set; }

    /// <summary>
    /// Immutable after creation. There is no domain operation that changes
    /// ResumeRevisionId once the Application has been created.
    /// </summary>
    public Guid ResumeRevisionId { get; private set; }

    public ApplicationStatus Status { get; private set; }
    public DateTime? SubmissionDate { get; private set; }
    public string JobSource { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Company Company { get; private set; } = null!;
    public Vacancy? Vacancy { get; private set; }
    public ResumeRevision ResumeRevision { get; private set; } = null!;

    private readonly List<Interview> _interviews = new();
    public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();

    private Application() { }

    public static Application Create(Guid companyId, Guid resumeRevisionId, string jobSource, Guid? vacancyId = null)
        => new()
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            VacancyId = vacancyId,
            ResumeRevisionId = resumeRevisionId,
            Status = ApplicationStatus.Draft,
            JobSource = jobSource,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    /// <summary>
    /// Transitions the Application to a new status.
    /// When transitioning to Applied, the linked ResumeRevision is explicitly
    /// marked as Applied (read-only terminal state) via ResumeRevision.MarkAsApplied().
    /// This ensures the snapshot is locked and can no longer be modified directly.
    /// </summary>
    public void TransitionTo(ApplicationStatus newStatus)
    {
        ApplicationStatusMachine.ValidateTransition(Status, newStatus);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        if (newStatus == ApplicationStatus.Applied)
        {
            SubmissionDate = DateTime.UtcNow;
            ResumeRevision.MarkAsApplied();
        }
    }
}

// Value Object: PersonalInfo (ADR 007 Recommendation)
public sealed record PersonalInfo(
    string FirstName,
    string LastName,
    string Email,
    string? Phone = null,
    string? LinkedInUrl = null,
    string? GitHubUrl = null,
    string? City = null,
    string? Country = null
);

// Resume (Aggregate Root — Career Profile Container)
public sealed class Resume
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ResumeTrack Track { get; private set; }
    public CareerLevel CareerLevel { get; private set; }
    public string TargetRole { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ResumeRevision> _revisions = new();
    public IReadOnlyCollection<ResumeRevision> Revisions => _revisions.AsReadOnly();

    private Resume() { }

    public static Resume Create(string name, ResumeTrack track, CareerLevel careerLevel, string targetRole) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Track = track,
        CareerLevel = careerLevel,
        TargetRole = targetRole,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates the first ResumeRevision (v1) under this Resume aggregate.
    /// Must be called as a separate explicit operation after Resume.Create().
    /// The resulting revision has Version = 1, Status = Draft, ParentRevisionId = null.
    /// </summary>
    public ResumeRevision CreateFirstRevision(string professionalSummary, PersonalInfo personalInfo)
    {
        if (_revisions.Count > 0)
            throw new DomainException("First revision already exists. Use SpawnNewVersion() on an existing revision.");

        var revision = ResumeRevision.CreateDraft(Id, professionalSummary, personalInfo);
        _revisions.Add(revision);
        UpdatedAt = DateTime.UtcNow;
        return revision;
    }
}

// ResumeRevision (Snapshot — Child Entity of Resume, Copy-on-Write)
public sealed class ResumeRevision
{
    public Guid Id { get; private set; }
    public Guid ResumeId { get; private set; }
    public RevisionStatus Status { get; private set; }
    public string ProfessionalSummary { get; private set; } = string.Empty;
    public PersonalInfo PersonalInfo { get; private set; } = null!;
    public string? FileReference { get; private set; }
    public int Version { get; private set; }
    public Guid? ParentRevisionId { get; private set; }
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

    private ResumeRevision() { }

    /// <summary>
    /// Internal factory for creating a first Draft revision.
    /// Called exclusively by Resume.CreateFirstRevision() to ensure aggregate root ownership.
    /// Version = 1, Status = Draft, ParentRevisionId = null, ResumeId = owning Resume.
    /// </summary>
    internal static ResumeRevision CreateDraft(Guid resumeId, string summary, PersonalInfo personalInfo) => new()
    {
        Id = Guid.NewGuid(),
        ResumeId = resumeId,
        Status = RevisionStatus.Draft,
        ProfessionalSummary = summary,
        PersonalInfo = personalInfo,
        Version = 1,
        ParentRevisionId = null,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    /// <summary>
    /// Copy-on-Write: creates a new Draft revision from this one.
    ///
    /// Required behavior — the new revision MUST:
    ///   - Receive a new Id (Guid.NewGuid())
    ///   - Remain under the same ResumeId
    ///   - Have Status = Draft
    ///   - Increment Version (Version + 1)
    ///   - Set ParentRevisionId to this revision's Id
    ///   - Set FileReference = null (PDF is NOT carried over)
    ///
    /// The implementation MUST deep-copy all owned snapshot collections:
    ///   - PersonalInfo           (value object — copied by value)
    ///   - ProfessionalSummary    (string — copied by value)
    ///   - WorkExperience         (owned child entities — each entry deep-copied with new Id)
    ///   - Education              (owned child entities — each entry deep-copied with new Id)
    ///   - Project                (owned child entities — each entry deep-copied with new Id)
    ///   - Language               (owned child entities — each entry deep-copied with new Id)
    ///   - ResumeRevisionSkill    (join entities — each entry deep-copied with new ResumeRevisionId)
    ///
    /// Each deep-copied child entity receives a new Id and the new revision's ResumeRevisionId.
    /// The original revision and its owned data remain completely unchanged.
    /// </summary>
    public ResumeRevision SpawnNewVersion()
    {
        var copy = new ResumeRevision
        {
            Id = Guid.NewGuid(),
            ResumeId = ResumeId,
            Status = RevisionStatus.Draft,
            ProfessionalSummary = ProfessionalSummary,
            PersonalInfo = PersonalInfo,
            FileReference = null,
            Version = Version + 1,
            ParentRevisionId = Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Deep-copy owned child entities — each receives a new Id + new ResumeRevisionId.
        // Implementation must iterate each collection and copy every entry.
        // WorkExperiences  → copy._workExperiences
        // Educations       → copy._educations
        // Projects         → copy._projects
        // Languages        → copy._languages
        // Skills           → copy._skills (with new ResumeRevisionId)

        return copy;
    }

    public void UpdateContent(string summary, PersonalInfo personalInfo)
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException("Cannot modify an Applied ResumeRevision. Create a new version instead.");
        ProfessionalSummary = summary;
        PersonalInfo = personalInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFileReference(string fileReference)
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException("Cannot modify file reference on an Applied ResumeRevision.");
        FileReference = fileReference;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsApplied()
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException("ResumeRevision is already in Applied state.");
        Status = RevisionStatus.Applied;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSkill(Guid masterSkillId, int proficiencyLevel = 3)
    {
        if (Status == RevisionStatus.Applied)
            throw new DomainException("Cannot modify skills on an Applied ResumeRevision.");
        if (_skills.Any(s => s.MasterSkillId == masterSkillId))
            throw new DomainException("Skill already added to this revision.");
        _skills.Add(new ResumeRevisionSkill(Id, masterSkillId, proficiencyLevel));
    }
}

// Company
public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? Industry { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Vacancy> _vacancies = new();
    public IReadOnlyCollection<Vacancy> Vacancies => _vacancies.AsReadOnly();

    private Company() { }
    public static Company Create(string name, string? website = null) => new()
    {
        Id = Guid.NewGuid(), Name = name, Website = website, CreatedAt = DateTime.UtcNow
    };
}

// Vacancy
public sealed class Vacancy
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Url { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Vacancy() { }
    public static Vacancy Create(Guid companyId, string title, string? url = null) => new()
    {
        Id = Guid.NewGuid(), CompanyId = companyId, Title = title, Url = url, CreatedAt = DateTime.UtcNow
    };
}

// Interview
public sealed class Interview
{
    public Guid Id { get; private set; }
    public Guid ApplicationId { get; private set; }
    public InterviewType Type { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string? Notes { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Interview() { }
    public static Interview Schedule(Guid applicationId, InterviewType type, DateTime scheduledAt) => new()
    {
        Id = Guid.NewGuid(), ApplicationId = applicationId, Type = type,
        ScheduledAt = scheduledAt, CreatedAt = DateTime.UtcNow
    };
}

// ResumeRevisionSkill (Join Entity)
public sealed class ResumeRevisionSkill
{
    public Guid ResumeRevisionId { get; private set; }
    public Guid MasterSkillId { get; private set; }
    public int ProficiencyLevel { get; private set; } // 1-5

    public MasterSkill MasterSkill { get; private set; } = null!;

    internal ResumeRevisionSkill(Guid revisionId, Guid masterSkillId, int proficiency)
    {
        ResumeRevisionId = revisionId;
        MasterSkillId = masterSkillId;
        ProficiencyLevel = proficiency;
    }

    private ResumeRevisionSkill() { }
}

// Owned Child Entities (owned by ResumeRevision, deep-copied per SpawnNewVersion)

public sealed class WorkExperience
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty; // String snapshot, NOT FK to Company
    public string PositionTitle { get; private set; } = string.Empty;
    public int StartMonth { get; private set; }
    public int StartYear { get; private set; }
    public int? EndMonth { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsCurrentJob { get; private set; }
    public string? Description { get; private set; }
    public string? Achievements { get; private set; }
    public string? TechStack { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WorkExperience() { }
}

public sealed class Education
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string InstitutionName { get; private set; } = string.Empty;
    public string? Degree { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public int? StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Education() { }
}

public sealed class Project
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string ProjectName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Role { get; private set; }
    public string? RepositoryUrl { get; private set; }
    public string? LiveDemoUrl { get; private set; }
    public string? TechStack { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Project() { }
}

public sealed class Language
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string LanguageName { get; private set; } = string.Empty;
    public string? Proficiency { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Language() { }
}

// Domain Exception
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}