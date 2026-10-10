using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public static class InterviewMapping
{
    public static InterviewDto MapToDto(Interview interview) => new()
    {
        Id = interview.Id,
        ApplicationId = interview.ApplicationId,
        Type = interview.Type,
        ScheduledAt = interview.ScheduledAt,
        ConductedAt = interview.ConductedAt,
        Notes = interview.Notes,
        Feedback = interview.Feedback
    };
}
