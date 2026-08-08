using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Queries.GetVacancies;

/// <summary>
/// Query handler for retrieving vacancies with AsNoTracking().
/// </summary>
public sealed class GetVacanciesQueryHandler
    : IRequestHandler<GetVacanciesQuery, List<VacancyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVacanciesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VacancyDto>> Handle(
        GetVacanciesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Vacancies
            .AsNoTracking();

        if (request.CompanyId.HasValue && request.CompanyId.Value != Guid.Empty)
        {
            query = query.Where(v => v.CompanyId == request.CompanyId.Value);
        }

        var vacancies = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);

        return vacancies.Select(CreateVacancyCommandHandler.MapToDto).ToList();
    }
}
