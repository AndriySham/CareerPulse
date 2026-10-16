using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Features.Recruiters.Commands.CreateRecruiter;
using CareerPulse.Application.Features.Recruiters.Commands.DeleteRecruiter;
using CareerPulse.Application.Features.Recruiters.Commands.UpdateRecruiter;
using CareerPulse.Application.Features.Recruiters.Queries.GetRecruiterById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
///  REST API controller for Recruiter entity management.
/// </summary>
[Route("api/companies/{companyId:guid}/recruiters")]
[ApiController]
public class RecruitersController : ControllerBase
{
    private readonly ISender _mediator;

    public RecruitersController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a Recruiter by ID.
    /// </summary>
    [HttpGet("{recruiterId:guid}", Name = "GetRecruiterById")]
    [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecruiterDto>> GetById(
        Guid companyId,
        Guid recruiterId,
        CancellationToken ct)
    {
        var query = new GetRecruiterByIdQuery(companyId, recruiterId);
        var result = await _mediator.Send(query, ct);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new Recruiter entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecruiterDto>> Create(
        Guid companyId,
        [FromBody] CreateRecruiterDto dto,
        CancellationToken ct)
    {
        var command = new CreateRecruiterCommand(companyId, dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtRoute("GetRecruiterById", new { companyId, recruiterId = result.Id }, result);
    }

    /// <summary>
    /// Updates a Recruiter entity.
    /// </summary>
    [HttpPut("{recruiterId:guid}")]
    [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecruiterDto>> Update(
        Guid companyId,
        Guid recruiterId,
        [FromBody] UpdateRecruiterDto dto,
        CancellationToken ct)
    {
        var command = new UpdateRecruiterCommand(companyId, recruiterId, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete an existing Recruiter.
    /// </summary>
    [HttpDelete("{recruiterId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid companyId,
        Guid recruiterId,
        CancellationToken ct)
    {
        var command = new DeleteRecruiterCommand(companyId, recruiterId);
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
