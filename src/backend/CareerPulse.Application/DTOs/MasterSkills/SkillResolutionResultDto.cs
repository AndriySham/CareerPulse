using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.MasterSkills;

/// <summary>
/// Result DTO returned by Skill Normalization pipeline (ResolveSkillsQuery).
/// ADR 006 & ADR 007: Categorizes resolved vs unknown skills to support HITL workflows.
/// </summary>
public sealed class SkillResolutionResultDto
{
    public List<SkillResolutionItemDto> ResolvedSkills { get; init; } = [];
    public List<SkillResolutionItemDto> UnknownSkills { get; init; } = [];
    public List<SkillResolutionItemDto> AllResults { get; init; } = [];
}

/// <summary>
/// Represents the resolution status of an individual raw skill input.
/// </summary>
public sealed class SkillResolutionItemDto
{
    public string RawText { get; init; } = string.Empty;
    public bool IsResolved { get; init; }
    public MasterSkillDto? MasterSkill { get; init; }
    public SkillResolutionStatus ResolutionStatus { get; init; }
}
