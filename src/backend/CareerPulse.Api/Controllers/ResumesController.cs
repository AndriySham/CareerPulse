using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;
using CareerPulse.Application.Features.Resumes.Commands.SpawnResumeVersion;
using CareerPulse.Application.Features.Resumes.Commands.UpdateResumeDraft;
using CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisionById;
using CareerPulse.Application.Features.Resumes.Queries.GetResumeRevisions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for ResumeRevision management adhering to ADR 005 (Draft Immutability & Copy-on-Write)
/// and ADR 006 (Skill Normalization).
/// </summary>
[ApiController]
[Route("api/resumes")]
public class ResumesController : ControllerBase
{
    private readonly ISender _mediator;

    public ResumesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new ResumeRevision draft (Version 1, Status = Draft).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResumeRevisionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResumeRevisionDto>> Create(
        [FromBody] CreateResumeDraftDto dto,
        CancellationToken ct)
    {
        var command = new CreateResumeDraftCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates summary and skills of an existing draft revision.
    /// ADR 005: Modifications allowed ONLY in Draft status.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ResumeRevisionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeRevisionDto>> Update(
        Guid id,
        [FromBody] UpdateResumeDraftDto dto,
        CancellationToken ct)
    {
        var command = new UpdateResumeDraftCommand(id, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Spawns a new editable Draft revision (Version = Parent.Version + 1) from an existing revision.
    /// ADR 005: Copy-on-Write pattern.
    /// </summary>
    [HttpPost("{id:guid}/spawn")]
    [ProducesResponseType(typeof(ResumeRevisionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeRevisionDto>> SpawnVersion(
        Guid id,
        CancellationToken ct)
    {
        var command = new SpawnResumeVersionCommand(id);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves all ResumeRevisions with AsNoTracking().
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ResumeRevisionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ResumeRevisionDto>>> GetAll(CancellationToken ct)
    {
        var query = new GetResumeRevisionsQuery();
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single ResumeRevision by ID with AsNoTracking().
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResumeRevisionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeRevisionDto>> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetResumeRevisionByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
