using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// Input DTO for creating a new ResumeRevision draft (Version 1).
/// </summary>
public sealed class CreateResumeDraftDto
{
    public PersonalInfo PersonalInfo { get; init; } = null!;
    public string ProfessionalSummary { get; init; } = string.Empty;
    public List<ResumeSkillInputDto> Skills { get; init; } = [];
}
