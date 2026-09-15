using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Commands.UpdateMasterSkill;

public sealed class UpdateMasterSkillCommandHandler 
    : IRequestHandler<UpdateMasterSkillCommand, MasterSkillDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateMasterSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MasterSkillDto> Handle(
        UpdateMasterSkillCommand request, 
        CancellationToken cancellationToken)
    {
        var skill = await _context.MasterSkills
            .Include(x => x.Aliases)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken); 
        
        if (skill == null) 
        { 
            throw new ResourceNotFoundException($"MasterSkill with ID '{request.Id}' was not found"); 
        
        }
        var trimmedName = request.Dto.Name.Trim();

        var nameConflict = await _context.MasterSkills 
            .AnyAsync( x => x.Id != request.Id && 
                x.Name.ToLower() == trimmedName.ToLower(), 
            cancellationToken); 
        if (nameConflict) 
        { 
            throw new ConflictException($"New MasterSkill name '{request.Dto.Name}' already exists."); 
        }
        
        var nameConflictWithAlias = await _context.MasterSkillAliases 
            .AnyAsync( x => x.AliasName.ToLower() == trimmedName.ToLower(), 
            cancellationToken);
        
        if (nameConflictWithAlias) 
        { 
            throw new ConflictException($"New MasterSkill Name '{trimmedName}' conflicts with an existing Alias."); 
        } 
        
        foreach (var alias in request.Dto.Aliases) 
        { 
            if (string.IsNullOrWhiteSpace(alias)) 
            { 
                continue; 
            } 
            
            var trimmedAlias = alias.Trim(); 
            var conflictsWithSkillName = await _context.MasterSkills 
                .AnyAsync( x => x.Name.ToLower() == trimmedAlias.ToLower(), 
                cancellationToken); 
            
            var conflictsWithAlias = await _context.MasterSkillAliases 
                .AnyAsync( x => x.MasterSkillId != request.Id && 
                    x.AliasName.ToLower() == trimmedAlias.ToLower(), 
                cancellationToken); 
            
            if (conflictsWithSkillName || conflictsWithAlias) 
            { 
                throw new ConflictException( $"Alias '{trimmedAlias}' conflicts with an existing skill or alias."); 
            } 
        } 
        
        skill.Update( trimmedName, request.Dto.Category, request.Dto.Aliases); 
        await _context.SaveChangesAsync(cancellationToken); 

        return MasterSkillMapping.MapToDto(skill); 
    }
}
