using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.MasterSkills;

/// <summary>
/// DTO representing a normalized MasterSkill and its aliases.
/// </summary>
public sealed class MasterSkillDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public SkillCategory Category { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<string> Aliases { get; init; } = [];
}
