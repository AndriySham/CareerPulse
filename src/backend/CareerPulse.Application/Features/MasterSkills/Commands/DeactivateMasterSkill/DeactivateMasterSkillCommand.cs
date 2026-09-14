using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Commands.DeactivateMasterSkill;

/// <summary>
/// Command to deactivate a MasterSkill in the catalog.
/// </summary>
public sealed record DeactivateMasterSkillCommand(Guid Id) : IRequest;
