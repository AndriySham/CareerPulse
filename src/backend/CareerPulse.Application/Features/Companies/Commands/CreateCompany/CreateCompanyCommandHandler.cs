using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Companies.Commands.CreateCompany;

/// <summary>
/// Command handler for creating a Company entity.
/// </summary>
public sealed class CreateCompanyCommandHandler
    : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDto> Handle(
        CreateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var trimmedName = dto.Name?.Trim() ?? string.Empty;

        var existingCompany = await _context.Companies
            .AnyAsync(c => c.Name.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (existingCompany)
        {
            throw new DomainException($"Company with name '{trimmedName}' already exists.");
        }

        var company = Company.Create(trimmedName, dto.Website, dto.Industry);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(company);
    }

    public static CompanyDto MapToDto(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Website = company.Website,
        Industry = company.Industry,
        Notes = company.Notes,
        IsArchived = company.IsArchived,
        CreatedAt = company.CreatedAt,
        UpdatedAt = company.UpdatedAt
    };
}
