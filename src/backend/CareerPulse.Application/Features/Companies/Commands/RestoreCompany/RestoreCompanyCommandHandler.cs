using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Companies.Commands.RestoreCompany;

/// <summary>
/// Command handler for restore an existing Company entity.
/// </summary>
public sealed class RestoreCompanyCommandHandler 
    : IRequestHandler<RestoreCompanyCommand, CompanyDto>
{
    private readonly IApplicationDbContext _context;

    public RestoreCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDto> Handle(
        RestoreCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            throw new ResourceNotFoundException($"Company with ID '{request.Id}' was not found.");
        }

        company.Restore();

        await _context.SaveChangesAsync(cancellationToken);

        return CompanyMapping.MapToDto(company);
    }
}
