using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;

/// <summary>
/// Handler for ChangeApplicationStatusCommand.
/// Enforces ApplicationStatusMachine rules and ADR 005 ResumeRevision locking.
/// </summary>
public sealed class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, ApplicationDto>
{
    private readonly IApplicationDbContext _context;

    public ChangeApplicationStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationDto> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ResumeRevision)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (application == null)
        {
            throw new DomainException($"Application with ID {request.Id} was not found.");
        }

        var newStatus = request.Dto.NewStatus;

        // Transition status (ApplicationStatusMachine validates allowed transitions)
        application.TransitionTo(newStatus);

        if (!string.IsNullOrWhiteSpace(request.Dto.Notes))
        {
            application.UpdateNotes(request.Dto.Notes);
        }

        // ADR 005: When transitioning to Applied, lock the linked ResumeRevision
        if (newStatus == ApplicationStatus.Applied && application.ResumeRevision != null)
        {
            if (application.ResumeRevision.Status == RevisionStatus.Draft)
            {
                application.ResumeRevision.MarkAsApplied();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return SubmitApplicationCommandHandler.MapToDto(application);
    }
}
