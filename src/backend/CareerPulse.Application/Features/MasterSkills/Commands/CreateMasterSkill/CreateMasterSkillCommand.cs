using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Domain.Enums;
using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Commands.CreateMasterSkill;

/// <summary>
/// Command to create a new MasterSkill in the catalog with optional initial aliases.
/// </summary>
public sealed record CreateMasterSkillCommand(
    string Name,
    SkillCategory Category,
    List<string>? Aliases = null
) : IRequest<MasterSkillDto>;
