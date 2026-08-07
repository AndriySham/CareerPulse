using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Companies.Queries.GetCompanies;

/// <summary>
/// Query handler for retrieving companies with AsNoTracking().
/// </summary>
public sealed class GetCompaniesQueryHandler
    : IRequestHandler<GetCompaniesQuery, List<CompanyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyDto>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Companies
            .AsNoTracking();

        if (!request.IncludeArchived)
        {
            query = query.Where(c => !c.IsArchived);
        }

        var companies = await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return companies.Select(CreateCompanyCommandHandler.MapToDto).ToList();
    }
}
