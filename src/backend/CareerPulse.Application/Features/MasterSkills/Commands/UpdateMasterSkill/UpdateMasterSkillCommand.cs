using CareerPulse.Application.DTOs.MasterSkills;
using MediatR;

namespace CareerPulse.Application.Features.MasterSkills.Commands.UpdateMasterSkill;

/// <summary>
/// Command to update a MasterSkill.
/// </summary>
public sealed record UpdateMasterSkillCommand(Guid Id, UpdateMasterSkillDto Dto) : IRequest<MasterSkillDto>;
