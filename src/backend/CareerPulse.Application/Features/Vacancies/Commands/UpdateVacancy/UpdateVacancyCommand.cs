using CareerPulse.Application.DTOs.Vacancies;
using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;

/// <summary>
/// Command to update an existing Vacancy entity.
/// </summary>
public sealed record UpdateVacancyCommand(Guid Id, UpdateVacancyDto Dto) : IRequest<VacancyDto>;
