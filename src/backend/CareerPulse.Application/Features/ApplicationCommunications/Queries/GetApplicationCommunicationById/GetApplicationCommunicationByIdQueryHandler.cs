using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.ApplicationCommunications.Queries.GetApplicationCommunicationById;

/// <summary>
/// Query handler for retrieving a single ApplicationCommunication by ID with AsNoTracking.
/// </summary>
public sealed class GetApplicationCommunicationByIdQueryHandler
    : IRequestHandler<GetApplicationCommunicationByIdQuery, ApplicationCommunicationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationCommunicationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationCommunicationDto?> Handle(
        GetApplicationCommunicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var communication = await _context.ApplicationCommunications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationCommunicationId &&
                x.ApplicationId == request.ApplicationId,
            cancellationToken);
           
        return communication is null ? null : ApplicationCommunicationMapping.MapToDto(communication);
    }
}
