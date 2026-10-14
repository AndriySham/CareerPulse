using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.CreateApplicationCommunication;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.DeleteApplicationCommunication;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.UpdateApplicationCommunication;
using CareerPulse.Application.Features.ApplicationCommunications.Queries.GetApplicationCommunicationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for ApplicationCommunication entity management.
/// </summary>
[Route("api/applications/{applicationId:guid}/communications")]
[ApiController]
public class ApplicationCommunicationsController : ControllerBase
{
    private readonly ISender _mediator;

    public ApplicationCommunicationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets an ApplicationCommunication by ID with AsNoTracking.
    /// </summary>
    [HttpGet("{applicationCommunicationId:guid}")]
    [ProducesResponseType(typeof(ApplicationCommunicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationCommunicationDto>> GetById(
        Guid applicationId,
        Guid applicationCommunicationId,
        CancellationToken ct)
    {
        var query = new GetApplicationCommunicationByIdQuery(applicationId, applicationCommunicationId);
        var result = await _mediator.Send(query, ct);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new ApplicationCommunication entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationCommunicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationCommunicationDto>> Create(
        Guid applicationId,
        [FromBody] CreateApplicationCommunicationDto dto,
        CancellationToken ct)
    {
        var command = new CreateApplicationCommunicationCommand(applicationId, dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { applicationId, applicationCommunicationId = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing ApplicationCommunication entity.
    /// </summary>
    [HttpPut("{applicationCommunicationId:guid}")]
    [ProducesResponseType(typeof(ApplicationCommunicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationCommunicationDto>> Update(
        Guid applicationId,
        Guid applicationCommunicationId,
        [FromBody] UpdateApplicationCommunicationDto dto,
        CancellationToken ct)
    {
        var command = new UpdateApplicationCommunicationCommand(applicationId, applicationCommunicationId, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete an existing ApplicationCommunication entity.
    /// </summary>
    [HttpDelete("{applicationCommunicationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid applicationId,
        Guid applicationCommunicationId,
        CancellationToken ct)
    {
        var command = new DeleteApplicationCommunicationCommand(applicationId, applicationCommunicationId);
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
