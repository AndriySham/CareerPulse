using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Domain.Enums;
using MediatR;

namespace CareerPulse.Application.Features.Applications.Queries.GetApplications;

/// <summary>
/// Query to retrieve applications with optional status, vacancy, and company filtering for Kanban pipeline.
/// </summary>
public sealed record GetApplicationsQuery(
    ApplicationStatus? Status = null,
    Guid? VacancyId = null,
    Guid? CompanyId = null) : IRequest<List<ApplicationDto>>;
