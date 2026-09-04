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
        Url = vacancy.Url,
        PostedAt = vacancy.PostedAt,
        CreatedAt = vacancy.CreatedAt,
        UpdatedAt = vacancy.UpdatedAt
    };
}
