using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.AI;

/// <summary>
/// Ephemeral DTO returned by AI resume extraction endpoint.
/// ADR 007: NEVER persisted directly — requires HITL user confirmation.
/// Confidence metadata exists ONLY during the import workflow.
/// </summary>
public sealed class ResumeImportResultDto
{
    public ScoredField<string> ProfessionalSummary { get; init; } = new();
    public ScoredField<PersonalInfoDto> PersonalInfo { get; init; } = new();
    public List<ScoredSkillDto> ExtractedSkills { get; init; } = [];
    public List<string> RawWarnings { get; init; } = [];
}

/// <summary>
/// Wraps an AI-extracted value with a confidence score for HITL review.
/// ADR 007: Fields below 70% confidence are flagged as NeedsReview.
/// </summary>
public sealed class ScoredField<T>
{
    public T? Value { get; init; }
    public double ConfidenceScore { get; init; } // 0.0 to 100.0
    public bool NeedsReview => ConfidenceScore < 70.0;
    public string? ExtractionNote { get; init; }
}

/// <summary>
/// AI-extracted skill with resolution status against the MasterSkill catalog.
/// ADR 006 + ADR 007: Unresolved skills trigger the HITL skill resolution workflow.
/// </summary>
public sealed class ScoredSkillDto
{
    public string RawText { get; init; } = string.Empty;
    public Guid? ResolvedMasterSkillId { get; init; }
    public string? ResolvedMasterSkillName { get; init; }
    public double ConfidenceScore { get; init; }
    public SkillResolutionStatus ResolutionStatus { get; init; }
}

public enum SkillResolutionStatus
{
    AutoResolved,    // Matched via MasterSkill.Name or Alias
    NeedsUserInput,  // No match found — requires HITL decision
    Ignored          // User dismissed this skill
}

/// <summary>Ephemeral personal info extracted from PDF.</summary>
public sealed class PersonalInfoDto
{
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? LinkedIn { get; init; }
    public string? GitHub { get; init; }
    public string? Location { get; init; }
}
