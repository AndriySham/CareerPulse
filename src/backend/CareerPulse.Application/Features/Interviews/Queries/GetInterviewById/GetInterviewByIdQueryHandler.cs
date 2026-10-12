using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Interviews.Queries.GetInterviewById;

/// <summary>
/// Query handler for retrieving a single Interview by ID with AsNoTracking.
/// </summary>
public sealed class GetInterviewByIdQueryHandler
    : IRequestHandler<GetInterviewByIdQuery, InterviewDto?>
{
    private readonly IApplicationDbContext _context;

    public GetInterviewByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewDto?> Handle(
        GetInterviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var interview = await _context.Interviews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.InterviewId && 
                x.ApplicationId == request.ApplicationId, cancellationToken);

        return interview is null ? null : InterviewMapping.MapToDto(interview);
    }
}
