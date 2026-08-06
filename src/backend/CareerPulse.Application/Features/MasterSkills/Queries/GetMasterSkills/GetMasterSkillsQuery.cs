using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Domain.Enums;
using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Queries.GetMasterSkills;

/// <summary>
/// Query to retrieve MasterSkills from the catalog with optional filtering.
/// </summary>
public sealed record GetMasterSkillsQuery(
    SkillCategory? Category = null,
    bool IncludeInactive = false
) : IRequest<List<MasterSkillDto>>;
