using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.VacancyLanguageRequirement;

/// <summary>
/// Input DTO for updating an existing VacancyLanguageRequirement.
/// </summary>
public sealed class UpdateVacancyLanguageRequirementDto
{
    public string LanguageName { get; init; } = string.Empty;
    public VacancyLanguageProficiency? Proficiency { get; init; }
    public string? ProficiencyDescription { get; init; }
}
