using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Queries.GetMasterSkills;

/// <summary>
/// Query handler for retrieving MasterSkills catalog.
/// </summary>
public sealed class GetMasterSkillsQueryHandler
    : IRequestHandler<GetMasterSkillsQuery, List<MasterSkillDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMasterSkillsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MasterSkillDto>> Handle(
        GetMasterSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.MasterSkills
            .Include(s => s.Aliases)
            .AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(s => s.Category == request.Category.Value);
        }

        var skills = await query
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return skills.Select(s => new MasterSkillDto
        {
            Id = s.Id,
            Name = s.Name,
            Category = s.Category,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            Aliases = s.Aliases.Select(a => a.AliasName).ToList()
        }).ToList();
    }
}
