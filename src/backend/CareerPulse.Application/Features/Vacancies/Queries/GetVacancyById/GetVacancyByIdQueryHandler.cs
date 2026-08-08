using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Queries.GetVacancyById;

/// <summary>
/// Query handler for retrieving a single Vacancy by ID with AsNoTracking().
/// </summary>
public sealed class GetVacancyByIdQueryHandler
    : IRequestHandler<GetVacancyByIdQuery, VacancyDto?>
{
    private readonly IApplicationDbContext _context;

    public GetVacancyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyDto?> Handle(
        GetVacancyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        return vacancy == null ? null : CreateVacancyCommandHandler.MapToDto(vacancy);
    }
}
