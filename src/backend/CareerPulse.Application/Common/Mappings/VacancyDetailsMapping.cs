using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.DTOs.VacancyLanguageRequirement;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public sealed class VacancyDetailsMapping
{
    public static VacancyDetailsDto MapToDto(Vacancy vacancy) => new()
    {
        Id = vacancy.Id,
        CompanyId = vacancy.CompanyId,
        Title = vacancy.Title,
        Description = vacancy.Description,
        Responsibilities = vacancy.Responsibilities,
        Requirements = vacancy.Requirements,
        NiceToHave = vacancy.NiceToHave,
        Benefits = vacancy.Benefits,
        Url = vacancy.Url,
        Location = vacancy.Location,
        WorkMode = vacancy.WorkMode,
        EmploymentType = vacancy.EmploymentType,
        SalaryMin = vacancy.SalaryMin,
        SalaryMax = vacancy.SalaryMax,
        SalaryCurrency = vacancy.SalaryCurrency,
        PostedAt = vacancy.PostedAt,
        CreatedAt = vacancy.CreatedAt,
        UpdatedAt = vacancy.UpdatedAt,

        LanguageRequirements = vacancy.LanguageRequirements.Select(x => new VacancyLanguageRequirementDto
        {
            Id = x.Id,
            LanguageName = x.LanguageName,
            Proficiency = x.Proficiency,
            ProficiencyDescription = x.ProficiencyDescription
        }).ToList()
    };
}
