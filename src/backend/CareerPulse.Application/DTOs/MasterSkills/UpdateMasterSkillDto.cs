using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.MasterSkills;

public sealed class UpdateMasterSkillDto
{
    public string Name { get; init; } = string.Empty;
    public SkillCategory Category { get; init; }
    public List<string> Aliases { get; init; } = [];
}
