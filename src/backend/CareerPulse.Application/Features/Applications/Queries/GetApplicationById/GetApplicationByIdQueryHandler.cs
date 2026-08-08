using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Applications.Queries.GetApplicationById;

/// <summary>
/// Query handler for retrieving a single Application by ID.
/// </summary>
public sealed class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, ApplicationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationDto?> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Company)
            .Include(a => a.Vacancy)
            .Include(a => a.ResumeRevision)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        return application == null ? null : SubmitApplicationCommandHandler.MapToDto(application);
    }
}
