using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public sealed class CompanyDetailsMapping
{
    public static CompanyDetailsDto MapToDto(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Website = company.Website,
        Industry = company.Industry,
        Notes = company.Notes,
        IsArchived = company.IsArchived,
        CreatedAt = company.CreatedAt,
        UpdatedAt = company.UpdatedAt,
        
        Recruiters = company.Recruiters.Select(x => new RecruiterDto
        {
            Id = x.Id,
            Name = x.Name,
            Phone = x.Phone,
            Email = x.Email,
            LinkedInUrl = x.LinkedInUrl,
            TelegramUrl = x.TelegramUrl,
            Notes = x.Notes
        }).ToList()
    };
}
