using CareerPulse.Application.DTOs.MasterSkills;
using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Queries.ResolveSkills;

/// <summary>
/// Query to resolve raw skill names against the MasterSkill catalog & MasterSkillAlias mappings.
/// ADR 006 & ADR 007: Identifies canonical skills and unknown skills for Human-in-the-Loop workflows.
/// </summary>
public sealed record ResolveSkillsQuery(
    IReadOnlyCollection<string> RawSkills
) : IRequest<SkillResolutionResultDto>;
