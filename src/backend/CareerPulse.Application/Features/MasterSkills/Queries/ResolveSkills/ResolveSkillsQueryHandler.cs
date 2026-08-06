using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Queries.ResolveSkills;

/// <summary>
/// Query handler for Skill Normalization pipeline (ADR 006).
/// Resolves raw skill strings against canonical MasterSkills and MasterSkillAliases.
/// Ephemeral read-only query that supports Human-in-the-Loop workflows (ADR 007).
/// </summary>
public sealed class ResolveSkillsQueryHandler
    : IRequestHandler<ResolveSkillsQuery, SkillResolutionResultDto>
{
    private readonly IApplicationDbContext _context;

    public ResolveSkillsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillResolutionResultDto> Handle(
        ResolveSkillsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RawSkills == null || request.RawSkills.Count == 0)
        {
            return new SkillResolutionResultDto();
        }

        // Fetch all active MasterSkills with their Aliases for in-memory normalization
        var masterSkills = await _context.MasterSkills
            .Include(s => s.Aliases)
            .Where(s => s.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Build lookup dictionaries for O(1) matching:
        // 1. Direct canonical name lookup (case-insensitive)
        var nameMap = masterSkills.ToDictionary(
            s => s.Name.Trim(),
            s => s,
            StringComparer.OrdinalIgnoreCase);

        // 2. Alias lookup (case-insensitive)
        var aliasMap = new Dictionary<string, MasterSkill>(StringComparer.OrdinalIgnoreCase);
        foreach (var skill in masterSkills)
        {
            foreach (var alias in skill.Aliases)
            {
                var trimmedAlias = alias.AliasName.Trim();
                if (!aliasMap.ContainsKey(trimmedAlias))
                {
                    aliasMap[trimmedAlias] = skill;
                }
            }
        }

        var resolvedList = new List<SkillResolutionItemDto>();
        var unknownList = new List<SkillResolutionItemDto>();
        var allList = new List<SkillResolutionItemDto>();

        foreach (var rawSkill in request.RawSkills)
        {
            if (string.IsNullOrWhiteSpace(rawSkill))
                continue;

            var trimmedRaw = rawSkill.Trim();
            MasterSkill? matchedSkill = null;

            if (nameMap.TryGetValue(trimmedRaw, out matchedSkill) ||
                aliasMap.TryGetValue(trimmedRaw, out matchedSkill))
            {
                var item = new SkillResolutionItemDto
                {
                    RawText = rawSkill,
                    IsResolved = true,
                    ResolutionStatus = SkillResolutionStatus.AutoResolved,
                    MasterSkill = new MasterSkillDto
                    {
                        Id = matchedSkill.Id,
                        Name = matchedSkill.Name,
                        Category = matchedSkill.Category,
                        IsActive = matchedSkill.IsActive,
                        CreatedAt = matchedSkill.CreatedAt,
                        Aliases = matchedSkill.Aliases.Select(a => a.AliasName).ToList()
                    }
                };

                resolvedList.Add(item);
                allList.Add(item);
            }
            else
            {
                var item = new SkillResolutionItemDto
                {
                    RawText = rawSkill,
                    IsResolved = false,
                    ResolutionStatus = SkillResolutionStatus.NeedsUserInput,
                    MasterSkill = null
                };

                unknownList.Add(item);
                allList.Add(item);
            }
        }

        return new SkillResolutionResultDto
        {
            ResolvedSkills = resolvedList,
            UnknownSkills = unknownList,
            AllResults = allList
        };
    }
}
