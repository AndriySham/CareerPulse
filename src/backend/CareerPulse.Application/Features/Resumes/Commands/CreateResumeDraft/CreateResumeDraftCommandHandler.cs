using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;

public sealed class CreateResumeDraftCommandHandler
    : IRequestHandler<CreateResumeDraftCommand, ResumeRevisionDto>
{
    private readonly IApplicationDbContext _context;

    public CreateResumeDraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeRevisionDto> Handle(
        CreateResumeDraftCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Verify MasterSkills exist in database catalog (ADR 006)
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

        var revision = ResumeRevision.CreateDraft(dto.PersonalInfo, dto.ProfessionalSummary);

        foreach (var skillInput in dto.Skills)
        {
            revision.AddSkill(skillInput.MasterSkillId, skillInput.ProficiencyLevel);
        }

        _context.ResumeRevisions.Add(revision);
        await _context.SaveChangesAsync(cancellationToken);

        // Fetch created entity with MasterSkill navigation properties loaded
        var created = await _context.ResumeRevisions
            .Include(r => r.Skills)
                .ThenInclude(s => s.MasterSkill)
            .AsNoTracking()
            .FirstAsync(r => r.Id == revision.Id, cancellationToken);

        return MapToDto(created);
    }

    internal static ResumeRevisionDto MapToDto(ResumeRevision revision)
    {
        return new ResumeRevisionDto
        {
            Id = revision.Id,
            Status = revision.Status,
            PersonalInfo = revision.PersonalInfo,
            ProfessionalSummary = revision.ProfessionalSummary,
            FileReference = revision.FileReference,
            Version = revision.Version,
            ParentRevisionId = revision.ParentRevisionId,
            CreatedAt = revision.CreatedAt,
            UpdatedAt = revision.UpdatedAt,
            Skills = revision.Skills.Select(s => new ResumeRevisionSkillDto
            {
                MasterSkillId = s.MasterSkillId,
                SkillName = s.MasterSkill?.Name ?? string.Empty,
                Category = s.MasterSkill?.Category ?? Domain.Enums.SkillCategory.Other,
                ProficiencyLevel = s.ProficiencyLevel
            }).ToList()
        };
    }
}
