using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Commands.SubmitApplication;

/// <summary>
/// Handler for SubmitApplicationCommand.
/// Enforces ADR 005 (ResumeRevision locking on submission) and domain invariants.
/// </summary>
public sealed class SubmitApplicationCommandHandler : IRequestHandler<SubmitApplicationCommand, ApplicationDto>
{
    private readonly IApplicationDbContext _context;

    public SubmitApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationDto> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == dto.CompanyId, cancellationToken);
        if (!companyExists)
        {
            throw new DomainException($"Company with ID {dto.CompanyId} was not found.");
        }

        var resumeRevision = await _context.ResumeRevisions
            .FirstOrDefaultAsync(r => r.Id == dto.ResumeRevisionId, cancellationToken);
        if (resumeRevision == null)
        {
            throw new DomainException($"ResumeRevision with ID {dto.ResumeRevisionId} was not found.");
        }

        if (dto.VacancyId.HasValue && dto.VacancyId.Value != Guid.Empty)
        {
            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v => v.Id == dto.VacancyId.Value, cancellationToken);
            if (vacancy == null)
            {
                throw new DomainException($"Vacancy with ID {dto.VacancyId.Value} was not found.");
            }
            if (vacancy.CompanyId != dto.CompanyId)
            {
                throw new DomainException($"Vacancy {dto.VacancyId.Value} does not belong to Company {dto.CompanyId}.");
            }
        }

        var application = Domain.Entities.Application.Create(
            dto.CompanyId,
            dto.ResumeRevisionId,
            dto.JobSource,
            dto.VacancyId);

        if (!string.IsNullOrWhiteSpace(dto.Notes))
        {
            application.UpdateNotes(dto.Notes);
        }

        if (dto.SubmitImmediately)
        {
            application.TransitionTo(ApplicationStatus.Applied);
            if (resumeRevision.Status == RevisionStatus.Draft)
            {
                resumeRevision.MarkAsApplied();
            }
        }

        _context.Applications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ResumeRevision)
            .AsNoTracking()
            .FirstAsync(a => a.Id == application.Id, cancellationToken);

        return MapToDto(created);
    }

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
