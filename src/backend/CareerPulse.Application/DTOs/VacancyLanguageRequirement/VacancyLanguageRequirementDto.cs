using CareerPulse.Domain.Enums;

namespace CareerPulse.Application.DTOs.VacancyLanguageRequirement;

public sealed class VacancyLanguageRequirementDto
{
    public Guid Id { get; init; }
    public string LanguageName { get; init; } = string.Empty;
    public VacancyLanguageProficiency? Proficiency { get; init; }
    public string? ProficiencyDescription { get; init; }
}
