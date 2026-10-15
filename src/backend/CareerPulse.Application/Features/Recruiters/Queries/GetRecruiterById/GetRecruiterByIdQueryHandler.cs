using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Recruiters.Queries.GetRecruiterById;

/// <summary>
/// Query handler for retrieving a single Recruiter by ID with AsNoTracking.
/// </summary>
public sealed class CreateRecruiterByIdQueryHandler
    : IRequestHandler<GetRecruiterByIdQuery, RecruiterDto?>
{
    private readonly IApplicationDbContext _context;

    public CreateRecruiterByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecruiterDto?> Handle(
        GetRecruiterByIdQuery request,
        CancellationToken cancellationToken)
    {
        var recruiter = await _context.Recruiters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RecruiterId
                && x.CompanyId == request.CompanyId, cancellationToken);

        return recruiter is null ? null : RecruiterMapping.MapToDto(recruiter);
    }
}
