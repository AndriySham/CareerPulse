using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public static class ApplicationCommunicationMapping
{
    public static ApplicationCommunicationDto MapToDto(ApplicationCommunication communication) => new()
    {
        Id = communication.Id,
        OccurredAt = communication.OccurredAt,
        Notes = communication.Notes
    };
}
