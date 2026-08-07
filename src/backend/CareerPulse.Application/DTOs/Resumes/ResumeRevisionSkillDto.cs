using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// DTO representing a skill attached to a ResumeRevision.
/// Links to a canonical MasterSkill per ADR 006.
/// </summary>
public sealed class ResumeRevisionSkillDto
{
    public Guid MasterSkillId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public SkillCategory Category { get; init; }
    public int ProficiencyLevel { get; init; }
}
