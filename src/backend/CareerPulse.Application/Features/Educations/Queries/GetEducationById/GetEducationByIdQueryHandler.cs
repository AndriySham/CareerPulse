using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Educations.Queries.GetEducationById;

/// <summary>
/// Query handler for retrieving a single Education by ID with AsNoTracking().
/// </summary>
public class GetEducationByIdQueryHandler
    : IRequestHandler<GetEducationByIdQuery, EducationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetEducationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EducationDto?> Handle(
        GetEducationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        return education is null ? null : EducationMapping.MapToDto(education);
    }
}
