using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class EducationMapping
{
    public static EducationDto MapToDto(Education education) => new()
    {
        Id = education.Id,
        ResumeRevisionId = education.ResumeRevisionId,
        InstitutionName = education.InstitutionName,
        Description = education.Description,
        StartYear = education.StartYear,
        EndYear = education.EndYear
    };
}
