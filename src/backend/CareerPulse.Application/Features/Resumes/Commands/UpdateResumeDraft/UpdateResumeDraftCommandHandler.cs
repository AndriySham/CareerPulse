using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Resumes.Commands.UpdateResumeDraft;

public sealed class UpdateResumeDraftCommandHandler
    : IRequestHandler<UpdateResumeDraftCommand, ResumeRevisionDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateResumeDraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeRevisionDto> Handle(
        UpdateResumeDraftCommand request,
        CancellationToken cancellationToken)
    {
        var revision = await _context.ResumeRevisions
            .Include(r => r.Skills)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (revision == null)
        {
            throw new DomainException($"ResumeRevision with ID '{request.Id}' was not found.");
        }

        var dto = request.Dto;

        // Verify MasterSkills exist in catalog (ADR 006)
        if (dto.Skills.Count > 0)
        {
            var requestedSkillIds = dto.Skills.Select(s => s.MasterSkillId).Distinct().ToList();
            var existingSkillIds = await _context.MasterSkills
                .Where(s => requestedSkillIds.Contains(s.Id) && s.IsActive)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            var invalidIds = requestedSkillIds.Except(existingSkillIds).ToList();
            if (invalidIds.Count > 0)
            {
                throw new DomainException($"MasterSkill(s) with ID(s) [{string.Join(", ", invalidIds)}] do not exist or are inactive.");
            }
        }

        // Domain method EnsureDraft() is invoked inside entity updates (ADR 005)
        revision.UpdateSummary(dto.ProfessionalSummary);
        revision.UpdatePersonalInfo(dto.PersonalInfo);

        // Sync skills
        var targetSkillDict = dto.Skills.ToDictionary(s => s.MasterSkillId, s => s.ProficiencyLevel);
        var currentSkillIds = revision.Skills.Select(s => s.MasterSkillId).ToList();

        // 1. Remove skills not present in target
        var toRemove = currentSkillIds.Except(targetSkillDict.Keys).ToList();
        foreach (var skillId in toRemove)
        {
            revision.RemoveSkill(skillId);
        }

        // 2. Add or update skills in target
        foreach (var (skillId, proficiency) in targetSkillDict)
        {
            if (currentSkillIds.Contains(skillId))
            {
                revision.RemoveSkill(skillId);
            }
            revision.AddSkill(skillId, proficiency);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties
        var updated = await _context.ResumeRevisions
            .Include(r => r.Skills)
                .ThenInclude(s => s.MasterSkill)
            .AsNoTracking()
            .FirstAsync(r => r.Id == revision.Id, cancellationToken);

        return CreateResumeDraftCommandHandler.MapToDto(updated);
    }
}
