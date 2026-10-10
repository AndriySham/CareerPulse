using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Features.Applications.Queries.GetApplicationById;
using CareerPulse.Application.Features.Interviews.Commands.CreateInterview;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
///  REST API controller for Interview entity management.
/// </summary>
[Route("api/applications/{applicationId:guid}/interviews")]
[ApiController]
public class InterviewsController : ControllerBase
{
    private readonly ISender _mediator;

    public InterviewsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets an Interview by ID. Not implemented yet.
    /// </summary>
    [HttpGet("{interviewId:guid}")]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<InterviewDto> GetById(
        Guid applicationId,
        Guid interviewId)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Creates a new Interview entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InterviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InterviewDto>> Create(
        Guid applicationId,
        [FromBody] CreateInterviewDto dto,
        CancellationToken ct)
    {
        var command = new CreateInterviewCommand(applicationId, dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { applicationId, interviewId = result.Id}, result );
    }
}
