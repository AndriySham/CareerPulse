using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using CareerPulse.Application.Features.Applications.Queries.GetApplicationById;
using CareerPulse.Application.Features.Applications.Queries.GetApplications;
using CareerPulse.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for Application & Kanban Pipeline management.
/// Enforces ApplicationStatusMachine transitions and ADR 005 ResumeRevision locking.
/// </summary>
[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly ISender _mediator;

    public ApplicationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates and optionally submits a new job application.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationDto>> Submit(
        [FromBody] SubmitApplicationDto dto,
        CancellationToken ct)
    {
        var command = new SubmitApplicationCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates application status in the Kanban pipeline according to state machine rules.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationDto>> ChangeStatus(
        Guid id,
        [FromBody] ChangeApplicationStatusDto dto,
        CancellationToken ct)
    {
        var command = new ChangeApplicationStatusCommand(id, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets all applications for Kanban board view, with optional status, vacancy, and company filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApplicationDto>>> GetAll(
        [FromQuery] ApplicationStatus? status,
        [FromQuery] Guid? vacancyId,
        [FromQuery] Guid? companyId,
        CancellationToken ct = default)
    {
        var query = new GetApplicationsQuery(status, vacancyId, companyId);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single application by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetApplicationByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
