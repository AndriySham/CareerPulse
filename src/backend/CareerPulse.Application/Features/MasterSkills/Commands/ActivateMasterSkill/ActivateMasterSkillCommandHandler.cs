using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.MasterSkills.Commands.ActivateMasterSkill;

/// <summary>
/// Command handler for activate an existing Masterskill entity.
/// </summary>
public sealed class ActivateMasterSkillCommandHandler 
    : IRequestHandler<ActivateMasterSkillCommand>
{
    private readonly IApplicationDbContext _context;

    public ActivateMasterSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        ActivateMasterSkillCommand request,
        CancellationToken cancellationToken)
    {
        var skill = await _context.MasterSkills
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (skill == null)
        {
            throw new ResourceNotFoundException($"MasterSkill with ID '{request.Id}' was not found.");
        }

        skill.Activate();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
