using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Languages;

/// <summary>
/// Input DTO for creating a new Language.
/// </summary>
public sealed class CreateLanguageDto
{
    public Guid ResumeRevisionId { get; init; }
    public string LanguageName { get; init; } = string.Empty;
    public LanguageProficiency Proficiency { get; init; }
}
