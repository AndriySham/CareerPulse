using CareerPulse.Application.DTOs.Companies;
using MediatR;

namespace CareerPulse.Application.Features.Companies.Commands.UpdateCompany;

/// <summary>
/// Command to update an existing Company.
/// </summary>
public sealed record UpdateCompanyCommand(Guid Id, UpdateCompanyDto Dto) : IRequest<CompanyDto>;
