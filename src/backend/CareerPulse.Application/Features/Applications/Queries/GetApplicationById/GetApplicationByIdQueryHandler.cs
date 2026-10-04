using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Queries.GetApplicationById;

/// <summary>
/// Query handler for retrieving a single Application by ID.
/// </summary>
public sealed class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, ApplicationDetailsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationDetailsDto?> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ApplicationCommunications)
            .Include(a => a.Interviews)
            .Include(a => a.Recruiter)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        return application == null ? null : ApplicationDetailsMapping.MapToDto(application);
    }
}
