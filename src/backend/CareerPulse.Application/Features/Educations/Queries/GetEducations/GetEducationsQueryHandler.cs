using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Educations.Queries.GetEducations;

/// <summary>
/// Query handler for retrieving educations with AsNoTracking().
/// </summary>
public class GetEducationsQueryHandler 
    : IRequestHandler<GetEducationsQuery, List<EducationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEducationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationDto>> Handle(
        GetEducationsQuery request,
        CancellationToken cancellationToken)
    {
        var educations = await _context.Educations
            .AsNoTracking()
            .Where(x => x.ResumeRevisionId == request.ResumeRevisionId)
            .OrderByDescending(x => x.EndYear)
            .ToListAsync(cancellationToken);

        return educations
            .Select(EducationMapping.MapToDto)
            .ToList();
    }
}
