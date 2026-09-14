using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Commands.DeactivateMasterSkill;

/// <summary>
/// Command handler for deactivating an existing Masterskill entity.
/// </summary>
public sealed class DeactivateMasterSkillCommandHandler 
    : IRequestHandler<DeactivateMasterSkillCommand>
{
    private readonly IApplicationDbContext _context;

    public DeactivateMasterSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeactivateMasterSkillCommand request,
        CancellationToken cancellationToken)
    {
        var skill = await _context.MasterSkills
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (skill == null)
        {
            throw new ResourceNotFoundException($"MasterSkill with ID '{request.Id}' was not found");
        }

        skill.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
