using CareerPulse.Application.DTOs.Vacancies;
using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>
/// Command to create a new Vacancy.
/// </summary>
public sealed record CreateVacancyCommand(CreateVacancyDto Dto) : IRequest<VacancyDto>;
