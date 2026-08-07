using CareerPulse.Application.DTOs.Companies;
using MediatR;

namespace CareerPulse.Application.Features.Companies.Commands.CreateCompany;

/// <summary>
/// Command to create a new Company.
/// </summary>
public sealed record CreateCompanyCommand(CreateCompanyDto Dto) : IRequest<CompanyDto>
{
    public CreateCompanyCommand(string name, string? website = null, string? industry = null)
        : this(new CreateCompanyDto { Name = name, Website = website, Industry = industry })
    {
    }
}
