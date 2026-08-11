namespace CareerPulse.Domain.Entities;

/// <summary>
/// Join entity linking a ResumeRevision to a normalized MasterSkill.
/// ADR 006: All skills must reference a valid MasterSkill — no raw text skills.
/// </summary>
public sealed class ResumeRevisionSkill
{
    public Guid ResumeRevisionId { get; private set; }
    public Guid MasterSkillId { get; private set; }
    public int ProficiencyLevel { get; private set; } // 1–5

    // Navigation
    public MasterSkill MasterSkill { get; private set; } = null!;

    private ResumeRevisionSkill() { }

    internal ResumeRevisionSkill(Guid revisionId, Guid masterSkillId, int proficiencyLevel)
    {
        if (proficiencyLevel is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(proficiencyLevel), "Must be between 1 and 5.");

        ResumeRevisionId = revisionId;
        MasterSkillId = masterSkillId;
        ProficiencyLevel = proficiencyLevel;
    }

    internal ResumeRevisionSkill DeepCopy(Guid newRevisionId)
    {
        return new ResumeRevisionSkill(newRevisionId, MasterSkillId, ProficiencyLevel);
    }
}
