using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class CompanyMapping
{
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
