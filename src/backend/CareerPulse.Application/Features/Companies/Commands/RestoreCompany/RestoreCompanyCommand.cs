using CareerPulse.Application.DTOs.Companies;
using MediatR;

namespace CareerPulse.Application.Features.Companies.Commands.RestoreCompany;

/// <summary>
/// Command to restore an existing Company.
/// </summary>
public sealed record RestoreCompanyCommand(Guid Id) : IRequest<CompanyDto>;
