using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Resumes.Commands.SpawnResumeVersion;

public sealed class SpawnResumeVersionCommandHandler
    : IRequestHandler<SpawnResumeVersionCommand, ResumeRevisionDto>
{
    private readonly IApplicationDbContext _context;

    public SpawnResumeVersionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeRevisionDto> Handle(
        SpawnResumeVersionCommand request,
        CancellationToken cancellationToken)
    {
        var parentRevision = await _context.ResumeRevisions
            .Include(r => r.Skills)
            .FirstOrDefaultAsync(r => r.Id == request.ParentRevisionId, cancellationToken);

        if (parentRevision == null)
        {
            throw new DomainException($"Parent ResumeRevision with ID '{request.ParentRevisionId}' was not found.");
        }

        // Copy-on-Write: Spawns new version with Version = parent.Version + 1, Status = Draft
        var newRevision = parentRevision.SpawnNewVersion();

        // Clone parent skills
        foreach (var skill in parentRevision.Skills)
        {
            newRevision.AddSkill(skill.MasterSkillId, skill.ProficiencyLevel);
        }

        _context.ResumeRevisions.Add(newRevision);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.ResumeRevisions
            .Include(r => r.Skills)
                .ThenInclude(s => s.MasterSkill)
            .AsNoTracking()
            .FirstAsync(r => r.Id == newRevision.Id, cancellationToken);

        return CreateResumeDraftCommandHandler.MapToDto(created);
    }
}
