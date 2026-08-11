using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Domain.Entities;

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

    public static Resume Create(string name, ResumeTrack track, CareerLevel careerLevel, string targetRole)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Resume Name is required.");

        if (string.IsNullOrWhiteSpace(targetRole))
            throw new DomainException("TargetRole is required.");

        return new Resume
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Track = track,
            CareerLevel = careerLevel,
            TargetRole = targetRole.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Resume Name is required.");

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public ResumeRevision CreateFirstRevision(string professionalSummary, PersonalInfo personalInfo)
    {
        if (_revisions.Count > 0)
            throw new DomainException("First revision already exists. Use SpawnNewVersion() on an existing revision.");

        var revision = ResumeRevision.CreateDraft(Id, personalInfo, professionalSummary);
        _revisions.Add(revision);
        UpdatedAt = DateTime.UtcNow;
        return revision;
    }

    public ResumeRevision SpawnRevision(ResumeRevision parentRevision)
    {
        if (parentRevision.ResumeId != Id)
            throw new DomainException($"ResumeRevision {parentRevision.Id} does not belong to Resume {Id}.");

        var newRevision = parentRevision.SpawnNewVersion();
        _revisions.Add(newRevision);
        UpdatedAt = DateTime.UtcNow;
        return newRevision;
    }
}
