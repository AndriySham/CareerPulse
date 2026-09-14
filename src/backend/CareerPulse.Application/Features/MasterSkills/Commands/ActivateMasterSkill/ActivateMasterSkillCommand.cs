using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Commands.ActivateMasterSkill;

/// <summary>
/// Command to activate a MasterSkill in the catalog.
/// </summary>
public sealed record ActivateMasterSkillCommand(Guid Id) : IRequest;