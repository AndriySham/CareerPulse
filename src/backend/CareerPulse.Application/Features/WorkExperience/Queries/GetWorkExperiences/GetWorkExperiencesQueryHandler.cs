using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperiences;

/// <summary>
/// Query handler for retrieving WorkExperience with AsNoTraking().
/// </summary>
public sealed class GetWorkExperiencesQueryHandler
    : IRequestHandler<GetWorkExperiencesQuery, List<WorkExperienceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkExperiencesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkExperienceDto>> Handle(
        GetWorkExperiencesQuery request,
        CancellationToken cancellationToken)
    {
        var workExperiences = await _context.WorkExperiences
            .AsNoTracking()
            .Where(x => x.ResumeRevisionId == request.ResumeRevisionId)
            .OrderByDescending(x => x.StartYear)
            .ThenByDescending(x => x.StartMonth)
            .ToListAsync(cancellationToken);

        return workExperiences
            .Select(WorkExperienceMapping.MapToDto)
            .ToList();
    }
}
