using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Application.DTOs.Resumes;

/// <summary>
/// Input DTO for creating a new ResumeRevision draft (Version 1).
/// </summary>
public sealed class CreateResumeDraftDto
{
    public string Name { get; init; } = string.Empty;
    public ResumeTrack Track { get; init; }
    public CareerLevel CareerLevel { get; init; }
    public string TargetRole { get; init; } = string.Empty;
    public PersonalInfo PersonalInfo { get; init; } = null!;
    public string ProfessionalSummary { get; init; } = string.Empty;
    public List<ResumeSkillInputDto> Skills { get; init; } = [];
}
