using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// DTO representing a structured ResumeRevision snapshot.
/// SSOT is PostgreSQL per ADR 002.
/// </summary>
public sealed class ResumeRevisionDto
{
    public Guid Id { get; init; }
    public RevisionStatus Status { get; init; }
    public PersonalInfo PersonalInfo { get; init; } = null!;
    public string ProfessionalSummary { get; init; } = string.Empty;
    public string? FileReference { get; init; }
    public int Version { get; init; }
    public Guid? ParentRevisionId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<ResumeRevisionSkillDto> Skills { get; init; } = [];
}
