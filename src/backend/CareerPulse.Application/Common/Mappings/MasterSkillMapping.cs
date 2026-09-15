using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class MasterSkillMapping
{
    public static MasterSkillDto MapToDto(MasterSkill skill) => new()
    {
        Id = skill.Id,
        Name = skill.Name,
        Category = skill.Category,
        IsActive = skill.IsActive,
        CreatedAt = skill.CreatedAt,
        Aliases = skill.Aliases.Select(a => a.AliasName).ToList()
    };
}
