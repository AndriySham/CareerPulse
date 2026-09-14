using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Commands.ActivateMasterSkill;

/// <summary>
/// Command to activate a MasterSkill in the catalog with optional initial aliases.
/// </summary>
public sealed record ActivateMasterSkillCommand(Guid Id) : IRequest;