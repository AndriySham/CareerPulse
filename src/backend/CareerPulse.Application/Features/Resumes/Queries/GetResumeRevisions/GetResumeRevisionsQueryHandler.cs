using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisions;

public sealed class GetResumeRevisionsQueryHandler
    : IRequestHandler<GetResumeRevisionsQuery, List<ResumeRevisionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetResumeRevisionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ResumeRevisionDto>> Handle(
        GetResumeRevisionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ResumeRevisions
            .Include(r => r.Skills)
                .ThenInclude(s => s.MasterSkill)
            .AsNoTracking();

        if (request.Id.HasValue)
        {
            query = query.Where(r => r.Id == request.Id.Value);
        }

        var revisions = await query
            .OrderByDescending(r => r.Version)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return revisions.Select(CreateResumeDraftCommandHandler.MapToDto).ToList();
    }
}
