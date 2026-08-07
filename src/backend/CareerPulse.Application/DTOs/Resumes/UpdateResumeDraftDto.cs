using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// Input DTO for updating an existing ResumeRevision draft.
/// Enforces Draft Immutability per ADR 005.
/// </summary>
public sealed class UpdateResumeDraftDto
{
    public PersonalInfo PersonalInfo { get; init; } = null!;
    public string ProfessionalSummary { get; init; } = string.Empty;
    public List<ResumeSkillInputDto> Skills { get; init; } = [];
}
