using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class VacancyLanguageRequirement
{
    public Guid Id { get; private set; }
    public Guid VacancyId { get; private set; }
    public string LanguageName { get; private set; } = string.Empty;
    public VacancyLanguageProficiency? Proficiency { get; private set; }
    public string? ProficiencyDescription { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private VacancyLanguageRequirement() { }

    public static VacancyLanguageRequirement Create(
        Guid vacancyId,
        string languageName,
        VacancyLanguageProficiency? proficiency,
        string? proficiencyDescription)
    {
        if (vacancyId == Guid.Empty)
            throw new DomainException("Vacancy ID is required.");

        if (string.IsNullOrWhiteSpace(languageName))
            throw new DomainException("LanguageName is required.");

        return new VacancyLanguageRequirement
        {
            Id = Guid.NewGuid(),
            VacancyId = vacancyId,
            LanguageName = languageName.Trim(),
            Proficiency = proficiency,
            ProficiencyDescription = proficiencyDescription?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
