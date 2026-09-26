using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperienceById;

/// <summary>
/// Query handler for retrieving a single WorkExperience by ID with AsNoTracking().
/// </summary>
public sealed class GetWorkExperienceByIdQueryHandler
    : IRequestHandler<GetWorkExperienceByIdQuery, WorkExperienceDto?>
{
    private readonly IApplicationDbContext _context;

    public GetWorkExperienceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkExperienceDto?> Handle(
        GetWorkExperienceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return workExperience is null ? null : WorkExperienceMapping.MapToDto(workExperience);
    }
}
