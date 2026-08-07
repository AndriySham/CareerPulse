using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisionById;

public sealed class GetResumeRevisionByIdQueryHandler
    : IRequestHandler<GetResumeRevisionByIdQuery, ResumeRevisionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetResumeRevisionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeRevisionDto?> Handle(
        GetResumeRevisionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var revision = await _context.ResumeRevisions
            .Include(r => r.Skills)
                .ThenInclude(s => s.MasterSkill)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return revision == null ? null : CreateResumeDraftCommandHandler.MapToDto(revision);
    }
}
