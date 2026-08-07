namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// Input DTO for attaching a MasterSkill to a ResumeRevision draft.
/// </summary>
public sealed class ResumeSkillInputDto
{
    public Guid MasterSkillId { get; init; }
    public int ProficiencyLevel { get; init; } = 3;
}
