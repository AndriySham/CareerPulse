using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Features.MasterSkills.Commands.CreateMasterSkill;
using CareerPulse.Application.Features.MasterSkills.Queries.GetMasterSkills;
using CareerPulse.Application.Features.MasterSkills.Queries.ResolveSkills;
using CareerPulse.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for MasterSkill catalog management and Skill Normalization (ADR 006).
/// </summary>
[ApiController]
[Route("api/master-skills")]
public class MasterSkillsController : ControllerBase
{
    private readonly ISender _mediator;

    public MasterSkillsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new MasterSkill in the catalog with optional initial aliases.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MasterSkillDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MasterSkillDto>> Create(
        [FromBody] CreateMasterSkillCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets MasterSkills from the catalog with optional category or active status filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<MasterSkillDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MasterSkillDto>>> GetAll(
        [FromQuery] SkillCategory? category,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var query = new GetMasterSkillsQuery(category, includeInactive);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single MasterSkill by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MasterSkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MasterSkillDto>> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetMasterSkillsQuery(IncludeInactive: true);
        var skills = await _mediator.Send(query, ct);
        var skill = skills.FirstOrDefault(s => s.Id == id);
        if (skill == null)
        {
            return NotFound();
        }
        return Ok(skill);
    }

    /// <summary>
    /// Resolves raw skill names against the MasterSkill catalog & aliases (ADR 006).
    /// Ephemeral read-only query supporting Human-in-the-Loop workflows (ADR 007).
    /// </summary>
    [HttpPost("resolve")]
    [ProducesResponseType(typeof(SkillResolutionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SkillResolutionResultDto>> Resolve(
        [FromBody] ResolveSkillsQuery query,
        CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
}
