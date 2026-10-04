using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.DTOs.Recruiters;

namespace CareerPulse.Application.Common.Mappings;

public sealed class ApplicationDetailsMapping
{
    public static ApplicationDetailsDto MapToDto(Domain.Entities.Application app) => new()
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
        AllowedTransitions = app.GetAllowedTransitions().ToList(),

        Communications = app.ApplicationCommunications.Select(x => new ApplicationCommunicationDto
        {
            Id = x.Id,
            OccurredAt = x.OccurredAt,
            Notes = x.Notes
        }).ToList(),

        Interviews = app.Interviews.Select(x => new InterviewDto
        {
            Id = x.Id,
            Type = x.Type,
            ScheduledAt = x.ScheduledAt,
            ConductedAt = x.ConductedAt,
            Notes = x.Notes,
            Feedback = x.Feedback
        }).ToList(),

        Recruiter = app.Recruiter is null
            ? null
            : new RecruiterDto
            {
                Id = app.Recruiter.Id,
                CompanyId = app.Recruiter.CompanyId,
                Name = app.Recruiter.Name,
                Phone = app.Recruiter.Phone,
                Email = app.Recruiter.Email,
                LinkedInUrl = app.Recruiter.LinkedInUrl,
                TelegramUrl = app.Recruiter.TelegramUrl,
                Notes = app.Recruiter.Notes
            }
    };
}
