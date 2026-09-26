using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;
using CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperienceById;
using CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperiences;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for WorkExperience entity management.
/// </summary>
[Route("api/work-experience")]
[ApiController]
public class WorkExperiencesController : ControllerBase
{
    private readonly ISender _mediator;

    public WorkExperiencesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all WorkExperience for a specific resime revision.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkExperienceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WorkExperienceDto>>> GetAll(
        Guid resumeRevisionId,
        CancellationToken ct)
    {
        var query = new GetWorkExperiencesQuery(resumeRevisionId);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single WorkExperience by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkExperienceDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetWorkExperienceByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new WorkExperience entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkExperienceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkExperienceDto>> Create(
        CreateWorkExperienceDto dto,
        CancellationToken ct)
    {
        var command = new CreateWorkExperienceCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { Id = result.Id }, result);
    }
}
