using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public static class RecruiterMapping
{
    public static RecruiterDto MapToDto(Recruiter recruiter) => new()
    {
        Id = recruiter.Id,
        CompanyId = recruiter.CompanyId,
        Name = recruiter.Name,
        Phone = recruiter.Phone,
        Email = recruiter.Email,
        LinkedInUrl = recruiter.LinkedInUrl,
        TelegramUrl = recruiter.TelegramUrl,
        Notes = recruiter.Notes
    };
}
