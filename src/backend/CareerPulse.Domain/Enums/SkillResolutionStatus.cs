namespace CareerPulse.Domain.Enums;

/// <summary>
/// Resolution status for a skill against the MasterSkill catalog.
/// ADR 006 & ADR 007: Unresolved skills trigger the Human-in-the-Loop resolution workflow.
/// </summary>
public enum SkillResolutionStatus
{
    AutoResolved,    // Matched via MasterSkill.Name or MasterSkillAlias
    NeedsUserInput,  // No match found — requires HITL decision
    Ignored          // User dismissed this skill
}
