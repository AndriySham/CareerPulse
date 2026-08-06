using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Commands.CreateMasterSkill;

/// <summary>
/// Command handler for creating a MasterSkill entity.
/// </summary>
public sealed class CreateMasterSkillCommandHandler
    : IRequestHandler<CreateMasterSkillCommand, MasterSkillDto>
{
    private readonly IApplicationDbContext _context;

    public CreateMasterSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MasterSkillDto> Handle(
        CreateMasterSkillCommand request,
        CancellationToken cancellationToken)
    {
        var trimmedName = request.Name.Trim();

        // Check canonical name duplicate (case-insensitive)
        var existingSkill = await _context.MasterSkills
            .AnyAsync(s => s.Name.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (existingSkill)
        {
            throw new DomainException($"MasterSkill with name '{trimmedName}' already exists.");
        }

        // Check if name conflicts with an existing alias
        var existingAliasConflict = await _context.MasterSkillAliases
            .AnyAsync(a => a.AliasName.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (existingAliasConflict)
        {
            throw new DomainException($"Skill name '{trimmedName}' conflicts with an existing skill alias.");
        }

        var skill = MasterSkill.Create(trimmedName, request.Category);

        if (request.Aliases != null && request.Aliases.Count > 0)
        {
            foreach (var alias in request.Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias)) continue;
                var trimmedAlias = alias.Trim();

                // Check alias conflicts against skill names and existing aliases
                var aliasNameConflict = await _context.MasterSkills
                    .AnyAsync(s => s.Name.ToLower() == trimmedAlias.ToLower(), cancellationToken);
                var aliasConflict = await _context.MasterSkillAliases
                    .AnyAsync(a => a.AliasName.ToLower() == trimmedAlias.ToLower(), cancellationToken);

                if (aliasNameConflict || aliasConflict)
                {
                    throw new DomainException($"Alias '{trimmedAlias}' conflicts with an existing skill or alias.");
                }

                skill.AddAlias(trimmedAlias);
            }
        }

        _context.MasterSkills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return new MasterSkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            IsActive = skill.IsActive,
            CreatedAt = skill.CreatedAt,
            Aliases = skill.Aliases.Select(a => a.AliasName).ToList()
        };
    }
}
