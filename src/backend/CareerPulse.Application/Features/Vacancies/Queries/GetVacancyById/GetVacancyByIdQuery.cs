using CareerPulse.Application.DTOs.Vacancies;
using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Queries.GetVacancyById;

/// <summary>
/// Query to retrieve a single Vacancy by ID.
/// </summary>
public sealed record GetVacancyByIdQuery(Guid Id) : IRequest<VacancyDto?>;
