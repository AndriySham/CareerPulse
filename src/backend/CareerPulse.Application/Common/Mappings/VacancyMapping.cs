using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class VacancyMapping
{
    public static VacancyDto MapToDto(Vacancy vacancy) => new()
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
        UpdatedAt = vacancy.UpdatedAt
    };
}
