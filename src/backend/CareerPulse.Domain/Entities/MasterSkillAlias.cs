namespace CareerPulse.Domain.Entities;

/// <summary>
/// Alternative text representation of a MasterSkill.
/// ADR 006: Resolves textual variations (e.g., "EF" → "Entity Framework Core").
/// Never exposed directly in UI — internal normalization detail.
/// </summary>
public sealed class MasterSkillAlias
{
    public Guid Id { get; private set; }
    public Guid MasterSkillId { get; private set; }
    public string AliasName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public MasterSkill MasterSkill { get; private set; } = null!;

    private MasterSkillAlias() { }

    internal MasterSkillAlias(Guid masterSkillId, string aliasName)
    {
        Id = Guid.NewGuid();
        MasterSkillId = masterSkillId;
        AliasName = aliasName.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
