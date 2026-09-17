using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Features.Educations.Commands.CreateEducation;
using CareerPulse.Application.Features.Educations.Commands.DeleteEducation;
using CareerPulse.Application.Features.Educations.Commands.UpdateEducation;
using CareerPulse.Application.Features.Educations.Queries.GetEducationById;
using CareerPulse.Application.Features.Educations.Queries.GetEducations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for Education entity management.
/// </summary>
[Route("api/educations")]
[ApiController]
public class EducationsController : ControllerBase
{
    private readonly ISender _mediator;

    public EducationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all educations for a specific resume revision.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EducationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EducationDto>>> GetAll(
        [FromQuery] Guid resumeRevisionId,
        CancellationToken ct)
    {
        var query = new GetEducationsQuery(resumeRevisionId);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single Education by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EducationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EducationDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetEducationByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new Education entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EducationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EducationDto>> Create(
        [FromBody] CreateEducationDto dto,
        CancellationToken ct)
    {
        var command = new CreateEducationCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing Education.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EducationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EducationDto>> Update(
        Guid id,
        [FromBody] UpdateEducationDto dto,
        CancellationToken ct)
    {
        var command = new UpdateEducationCommand(id, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete an existing Education.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
    {
        var command = new DeleteEducationCommand(id);
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
