using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Languages;

/// <summary>
/// DTO representing a Language entity.
/// </summary>
public sealed class LanguageDto
{
    public Guid Id { get; init; }
    public Guid ResumeRevisionId { get; init; }
    public string LanguageName { get; init; } = string.Empty;
    public LanguageProficiency Proficiency { get; init; }
}
