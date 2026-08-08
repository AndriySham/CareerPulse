using CareerPulse.Application.DTOs.Vacancies;
using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Queries.GetVacancies;

/// <summary>
/// Query to retrieve vacancies, with optional filtering by company ID.
/// </summary>
public sealed record GetVacanciesQuery(Guid? CompanyId = null) : IRequest<List<VacancyDto>>;
