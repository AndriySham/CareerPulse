using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Companies.Commands.UpdateCompany;

/// <summary>
/// Command handler for updating an existing Company entity.
/// </summary>
public sealed class UpdateCompanyCommandHandler
    : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDto> Handle(
        UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            throw new DomainException($"Company with ID '{request.Id}' was not found.");
        }

        var dto = request.Dto;
        var trimmedName = dto.Name?.Trim() ?? string.Empty;

        var nameConflict = await _context.Companies
            .AnyAsync(c => c.Id != request.Id && c.Name.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (nameConflict)
        {
            throw new DomainException($"Company with name '{trimmedName}' already exists.");
        }

        company.Update(trimmedName, dto.Website, dto.Industry, dto.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return CreateCompanyCommandHandler.MapToDto(company);
    }
}
