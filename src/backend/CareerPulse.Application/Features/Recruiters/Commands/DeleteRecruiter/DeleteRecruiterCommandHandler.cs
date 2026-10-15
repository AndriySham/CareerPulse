using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Recruiters.Commands.DeleteRecruiter;

/// <summary>
/// Command handler for deleting an existing Recruiter Entity.
/// </summary>
public sealed class DeleteRecruiterCommandHandler : IRequestHandler<DeleteRecruiterCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteRecruiterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteRecruiterCommand request,
        CancellationToken cancellationToken)
    {
        var recruiter = await _context.Recruiters
            .FirstOrDefaultAsync(x => x.Id == request.RecruiterId
                && x.CompanyId == request.CompanyId, cancellationToken);
        if (recruiter == null)
        {
            throw new ResourceNotFoundException($"Resource with Company ID '{request.CompanyId}' and Recruiter ID '{request.RecruiterId}' was not found.");
        }

        var isLinkedToApplications = await _context.Applications
            .AnyAsync(x => x.RecruiterId == request.RecruiterId, cancellationToken);
        if (isLinkedToApplications)
        {
            throw new ConflictException($"Recruiter '{request.RecruiterId}' is linked to existing applications and cannot be deleted.");
        }

        _context.Recruiters.Remove(recruiter);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
