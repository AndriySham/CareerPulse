using MediatR;

namespace CareerPulse.Application.Features.Vacancies.Commands.DeleteVacancy;

/// <summary>
/// Command to delete an existing Vacancy.
/// </summary>
public sealed record DeleteVacancyCommand(Guid Id) : IRequest;
