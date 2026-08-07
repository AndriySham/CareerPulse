using CareerPulse.Application.DTOs.Companies;
using MediatR;

namespace CareerPulse.Application.Features.Companies.Queries.GetCompanies;

/// <summary>
/// Query to retrieve a list of companies with optional filter for archived items.
/// Uses AsNoTracking().
/// </summary>
public sealed record GetCompaniesQuery(bool IncludeArchived = false) : IRequest<List<CompanyDto>>;
