using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public static class WorkExperienceMapping
{
    public static WorkExperienceDto MapToDto(WorkExperience workExperience) => new()
    {
        Id = workExperience.Id,
        ResumeRevisionId = workExperience.ResumeRevisionId,
        CompanyName = workExperience.CompanyName,
        PositionTitle = workExperience.PositionTitle,
        StartMonth = workExperience.StartMonth,
        StartYear = workExperience.StartYear,
        EndMonth = workExperience.EndMonth,
        EndYear = workExperience.EndYear,
        IsCurrentJob = workExperience.IsCurrentJob,
        Description = workExperience.Description,
        Achievements = workExperience.Achievements,
        TechStack = workExperience.TechStack
    };
}
