using CareerPulse.Application.DTOs.Companies;
using MediatR;

namespace CareerPulse.Application.Features.Companies.Queries.GetCompanyById;

/// <summary>
/// Query to retrieve a single Company by ID with AsNoTracking().
/// </summary>
public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDto?>;
