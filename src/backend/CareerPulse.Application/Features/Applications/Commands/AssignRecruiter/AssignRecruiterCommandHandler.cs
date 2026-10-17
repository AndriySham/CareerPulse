using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Commands.AssignRecruiter;

/// <summary>
/// Handler for AssignRecruiterCommand.
/// Enforces company boundary and updates the Application's Recruiter reference.
/// </summary>
public sealed class AssignRecruiterCommandHandler : IRequestHandler<AssignRecruiterCommand, ApplicationDetailsDto>
{
    private readonly IApplicationDbContext _context;

    public AssignRecruiterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationDetailsDto> Handle(AssignRecruiterCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ResumeRevision)
            .Include(a => a.Recruiter)
            .Include(a => a.ApplicationCommunications)
            .Include(a => a.Interviews)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application == null)
        {
            throw new ResourceNotFoundException($"Application with ID '{request.ApplicationId}' was not found.");
        }

        if (request.RecruiterId.HasValue && request.RecruiterId.Value != Guid.Empty)
        {
            var recruiter = await _context.Recruiters
                .FirstOrDefaultAsync(r => r.Id == request.RecruiterId.Value, cancellationToken);

            if (recruiter == null)
            {
                throw new ResourceNotFoundException($"Recruiter with ID '{request.RecruiterId.Value}' was not found.");
            }

            if (recruiter.CompanyId != application.CompanyId)
            {
                throw new DomainException($"Recruiter '{recruiter.Id}' belongs to Company '{recruiter.CompanyId}', not Application Company '{application.CompanyId}'.");
            }

            application.AssignRecruiter(recruiter);
        }
        else
        {
            application.AssignRecruiter(null);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ApplicationDetailsMapping.MapToDto(application);
    }
}
