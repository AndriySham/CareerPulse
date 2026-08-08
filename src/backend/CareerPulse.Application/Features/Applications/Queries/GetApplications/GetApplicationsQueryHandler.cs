using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Queries.GetApplications;

/// <summary>
/// Query handler for retrieving Applications with AsNoTracking() for read operations.
/// </summary>
public sealed class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, List<ApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApplicationDto>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ResumeRevision)
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        if (request.VacancyId.HasValue && request.VacancyId.Value != Guid.Empty)
        {
            query = query.Where(a => a.VacancyId == request.VacancyId.Value);
        }

        if (request.CompanyId.HasValue && request.CompanyId.Value != Guid.Empty)
        {
            query = query.Where(a => a.CompanyId == request.CompanyId.Value);
        }

        var applications = await query
            .OrderByDescending(a => a.UpdatedAt)
            .ToListAsync(cancellationToken);

        return applications.Select(SubmitApplicationCommandHandler.MapToDto).ToList();
    }
}
