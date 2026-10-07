using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.VacancyLanguageRequirement;

/// <summary>
/// Input DTO for creating a new VacancyLanguageRequirement.
/// </summary>

public sealed class CreateVacancyLanguageRequirementDto
{
    public string LanguageName { get; init; } = string.Empty;
    public VacancyLanguageProficiency? Proficiency { get; init; }
    public string? ProficiencyDescription { get; init; }
}
