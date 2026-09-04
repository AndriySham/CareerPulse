using CareerPulse.Application.DTOs.Applications;

namespace CareerPulse.Application.Common.Mappings;

public class ApplicationMapping
{
    public static ApplicationDto MapToDto(Domain.Entities.Application app) => new()
    {
        Id = app.Id,
        CompanyId = app.CompanyId,
        CompanyName = app.Company?.Name ?? string.Empty,
        VacancyId = app.VacancyId,
        VacancyTitle = app.Vacancy?.Title,
        ResumeRevisionId = app.ResumeRevisionId,
        Status = app.Status,
        JobSource = app.JobSource,
        Notes = app.Notes,
        AppliedAt = app.SubmissionDate,
        CreatedAt = app.CreatedAt,
        UpdatedAt = app.UpdatedAt,
        AllowedTransitions = app.GetAllowedTransitions().ToList()
    };
}
