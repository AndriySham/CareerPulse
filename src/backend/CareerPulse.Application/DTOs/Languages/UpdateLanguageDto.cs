using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.Languages;

/// <summary>
///  Input DTO for updatinf an existing Language.
/// </summary>
public sealed class UpdateLanguageDto
{
    public string LanguageName { get; init; } = string.Empty;
    public LanguageProficiency Proficiency { get; init; }
}
