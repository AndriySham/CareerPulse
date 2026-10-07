using CareerPulse.Domain.Enums;

namespace CareerPulse.Domain.Models;

/// <summary>
/// Input for synchronizing a vacancy's language requirements.
/// </summary>
public sealed record LanguageRequirementInput(
    string LanguageName,
    VacancyLanguageProficiency? Proficiency,
    string? ProficiencyDescription
);
