using CareerPulse.Application.DTOs.Vacancies;
using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>
/// Command to create a new Vacancy.
/// </summary>
public sealed record CreateVacancyCommand(CreateVacancyDto Dto) : IRequest<VacancyDto>
{
    public CreateVacancyCommand(Guid companyId, string title, string? description = null, string? url = null, DateTime? postedAt = null)
        : this(new CreateVacancyDto { CompanyId = companyId, Title = title, Description = description, Url = url, PostedAt = postedAt })
    {
    }
}
